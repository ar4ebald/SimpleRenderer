using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core.Rendering
{
    static class InterpolatorFactory
    {
        const BindingFlags FieldsFlags = BindingFlags.Instance | BindingFlags.Public;
        const string ImplementationMethodName = "Invoke";

        static readonly ModuleBuilder _moduleBuilder;

        static readonly CustomAttributeBuilder _isReadonlyAttribute;

        static readonly FieldInfo _xField;
        static readonly FieldInfo _yField;
        static readonly FieldInfo _zField;

        static readonly IReadOnlyDictionary<Type, OpCode> _convertersByType;

        static InterpolatorFactory()
        {
            var an = new AssemblyName("InterpolatorsAssembly");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            _isReadonlyAttribute = new CustomAttributeBuilder(
                typeof(IsReadOnlyAttribute).GetConstructor(Type.EmptyTypes),
                new object[0]
            );

            _xField = typeof(Vector3).GetField("X");
            _yField = typeof(Vector3).GetField("Y");
            _zField = typeof(Vector3).GetField("Z");

            _convertersByType = new ReadOnlyDictionary<Type, OpCode>(new Dictionary<Type, OpCode>()
            {
                [typeof(double)] = OpCodes.Nop,
                [typeof(SByte)]  = OpCodes.Conv_I1,
                [typeof(Int16)]  = OpCodes.Conv_I2,
                [typeof(Int32)]  = OpCodes.Conv_I4,
                [typeof(Int64)]  = OpCodes.Conv_I8,
                [typeof(Byte)]   = OpCodes.Conv_U1,
                [typeof(UInt16)] = OpCodes.Conv_U2,
                [typeof(UInt32)] = OpCodes.Conv_U4,
                [typeof(UInt64)] = OpCodes.Conv_U8,
                [typeof(Single)] = OpCodes.Conv_R4
            });
        }

        public static Interpolator<T> CreateDelegate<T>() where T : unmanaged
        {
            var typeBuilder = _moduleBuilder.DefineType(
                $"{nameof(T)}_Interpolator",
                TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class |
                TypeAttributes.Sealed
            );

            var tRef = typeof(T).MakeByRefType();
            var vector3Ref = typeof(Vector3).MakeByRefType();

            var parameterTypes = new[] { tRef, tRef, tRef, vector3Ref };

            var method = typeBuilder.DefineMethod(
                ImplementationMethodName,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(T),
                parameterTypes
            );

            var v0 = method.DefineParameter(1, ParameterAttributes.In, "v0");
            var v1 = method.DefineParameter(2, ParameterAttributes.In, "v1");
            var v2 = method.DefineParameter(3, ParameterAttributes.In, "v2");

            var barycentric = method.DefineParameter(4, ParameterAttributes.In, "barycentric");

            v0.SetCustomAttribute(_isReadonlyAttribute);
            v1.SetCustomAttribute(_isReadonlyAttribute);
            v2.SetCustomAttribute(_isReadonlyAttribute);
            barycentric.SetCustomAttribute(_isReadonlyAttribute);

            var il = method.GetILGenerator();
            var output = il.DeclareLocal(typeof(T));

            var parentFields = new Stack<FieldInfo>();

            void InterpolateField(FieldInfo field)
            {
                var type = field.FieldType;

                if (!type.IsPrimitive)
                {
                    parentFields.Push(field);

                    foreach (var subField in field.FieldType.GetFields(FieldsFlags))
                        InterpolateField(subField);

                    return;
                }

                if (!_convertersByType.TryGetValue(type, out var convert))
                    throw new InvalidOperationException($"Can't cast field {field.Name} to double");

                void CalculateFieldPart(FieldInfo coord)
                {
                    foreach (var parent in parentFields)
                        il.Emit(OpCodes.Ldflda, parent);
                    il.Emit(OpCodes.Ldfld, field);
                    if (convert != OpCodes.Nop)
                        il.Emit(OpCodes.Conv_R8);

                    il.Emit(OpCodes.Ldarg_3);
                    il.Emit(OpCodes.Ldfld, coord);
                    il.Emit(OpCodes.Mul);
                }

                il.Emit(OpCodes.Ldloca_S, output);
                foreach (var parent in parentFields)
                    il.Emit(OpCodes.Ldflda, parent);

                il.Emit(OpCodes.Ldarg_0);
                CalculateFieldPart(_xField);

                il.Emit(OpCodes.Ldarg_1);
                CalculateFieldPart(_yField);
                il.Emit(OpCodes.Add);

                il.Emit(OpCodes.Ldarg_2);
                CalculateFieldPart(_zField);
                il.Emit(OpCodes.Add);

                if (convert != OpCodes.Nop)
                    il.Emit(convert);

                il.Emit(OpCodes.Stfld, field);
            }


            foreach (var field in typeof(T).GetFields(FieldsFlags))
                InterpolateField(field);

            il.Emit(OpCodes.Ldloc, output);
            il.Emit(OpCodes.Ret);


            var interpolatorClassType = typeBuilder.CreateType();
            var interpolatorMethod = interpolatorClassType.GetMethod(ImplementationMethodName);
            var interpolatorType = typeof(Interpolator<T>);

            return (Interpolator<T>)Delegate.CreateDelegate(interpolatorType, interpolatorMethod);
        }
    }
}