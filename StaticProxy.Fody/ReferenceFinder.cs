using System;
using System.Linq;

using Mono.Cecil;

namespace StaticProxy.Fody
{
    public class ReferenceFinder
    {
        private readonly ModuleDefinition moduleDefinition;

        public ReferenceFinder(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
        }

        public MethodReference GetMethodReference(Type declaringType, Func<MethodDefinition, bool> predicate)
        {
            return GetMethodReference(GetTypeReference(declaringType), predicate);
        }

        public MethodReference GetMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate)
        {
            var typeDefinition = typeReference.Resolve();

            MethodDefinition methodDefinition;
            do
            {
                methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
                typeDefinition = typeDefinition.BaseType == null ? null : typeDefinition.BaseType.Resolve();
            } while (methodDefinition == null && typeDefinition != null);

            return moduleDefinition.Import(methodDefinition);
        }

        public TypeReference GetTypeReference(Type type)
        {
            return moduleDefinition.Import(type);
        }
    }
}