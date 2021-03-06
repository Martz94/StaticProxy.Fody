﻿namespace StaticProxy.Fody.MethodWeaving
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;

    public static class MethodDefinitionExtensions
    {
        public static VariableDefinition AddVariableDefinition(this MethodDefinition method, string variableName, TypeReference variableType)
        {
            var variableDefinition = new VariableDefinition(variableName, variableType);
            method.Body.Variables.Add(variableDefinition);
            return variableDefinition;
        }

        public static MethodDefinition CreateCopy(this MethodDefinition templateMethod, string newMethodName)
        {
            var newMethod = new MethodDefinition(
                newMethodName,
                templateMethod.Attributes,
                templateMethod.ReturnType);

            CopyMethodData(templateMethod, newMethod);

            return newMethod;
        }

        public static MethodDefinition CreateImplementation(this MethodDefinition interfaceMethod)
        {
            var newMethod = new MethodDefinition(
                interfaceMethod.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final,
                interfaceMethod.ReturnType);

            newMethod.Overrides.Add(interfaceMethod);

            CopyMethodData(interfaceMethod, newMethod);

            return newMethod;
        }

        private static void CopyMethodData(MethodDefinition templateMethod, MethodDefinition newMethod)
        {
            foreach (var parameterDefinition in templateMethod.Parameters)
            {
                newMethod.Parameters.Add(parameterDefinition);
            }

            if (templateMethod.HasBody)
            {
                newMethod.Body.InitLocals = true;

                foreach (var variableDefinition in templateMethod.Body.Variables)
                {
                    newMethod.Body.Variables.Add(variableDefinition);
                }

                foreach (ExceptionHandler exceptionHandler in templateMethod.Body.ExceptionHandlers)
                {
                    newMethod.Body.ExceptionHandlers.Add(exceptionHandler);
                }

                foreach (var instruction in templateMethod.Body.Instructions)
                {
                    newMethod.Body.Instructions.Add(instruction);
                }

                newMethod.Body.OptimizeMacros();
            }
        }
    }
}