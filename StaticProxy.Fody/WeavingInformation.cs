﻿namespace StaticProxy.Fody
{

    using Mono.Cecil;

    public static class WeavingInformation
    {
        public static ModuleDefinition ModuleDefinition { get; private set; }

        public static TypeReference StaticProxyAttribute { get; private set; }

        public static ModuleDefinition InterceptorModuleDefinition { get; private set; }

        public static TypeReference DynamicInterceptorManagerReference { get; private set; }

        public static ReferenceFinder ReferenceFinder { get; private set; }

        public static TypeReference ObjectTypeReference { get; private set; }

        public static void Initialize()
        {
            ModuleDefinition = ModuleWeaver.Instance.ModuleDefinition;
            ReferenceFinder = new ReferenceFinder(ModuleDefinition);

            StaticProxyAttribute = ModuleDefinition.GetTypeReference("StaticProxyAttribute");

            InterceptorModuleDefinition = ResolveInterceptorModuleDefinition();

            TypeDefinition dynamicInterceptorManagerTypeDefinition = InterceptorModuleDefinition.GetTypeDefinition("IDynamicInterceptorManager");
            DynamicInterceptorManagerReference = ModuleDefinition.Import(dynamicInterceptorManagerTypeDefinition);

            ObjectTypeReference = ReferenceFinder.GetTypeReference(typeof(object));
        }

        public static bool IsStaticProxyAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType == StaticProxyAttribute;
        }

        // todo determine how to find the "StaticProxy.Interceptor" module in case it's not referenced by the project!!
        // maybe use the add in path - it points to the packages/StaticProxy.Fody path...
        private static ModuleDefinition ResolveInterceptorModuleDefinition()
        {
            AssemblyDefinition definition = ModuleWeaver.Instance.AssemblyResolver.Resolve("StaticProxy.Interceptor");
            if (definition == null)
            {
                throw new WeavingException("can't find StaticProxy.Interceptor module. Make sure you've downloaded and installed the nuget package!");
            }

            return definition.MainModule;
        }
    }
}