using MultiProcessCommunicator.Client;
using MultiProcessCommunicator.Server;
using System.Collections.Generic;
using System.Reflection;

namespace MultiProcessCommunicator
{
    public static class MpcManager
    {
        public static T CreateClient<T>() where T : IMpcService
        {
            var client = DispatchProxy.Create<T, ClientDecorator>();

            return client;
        }

        private readonly static Dictionary<int, ServerInstance> ServerInstances = new Dictionary<int, ServerInstance>();

        internal static ServerInstance GetService(int serviceId)
        {
            ServerInstance instance = null;
            ServerInstances.TryGetValue(serviceId, out instance);
            return instance;
        }

        public static MpcServer CreateServer<T>(IMpcService service, int port)
        {
            var interfaceType = typeof(T);
            var serviceId = interfaceType.Name.GetHashCode();
            var serverInstance = new ServerInstance(port) { Instance = service };
            var serverInstances = new Dictionary<int, ServerInstance>();

            serverInstances.Add(serviceId, serverInstance);

            foreach (var serviceInterface in interfaceType.GetInterfaces())
            {
                if (serviceInterface == typeof(IMpcService))
                    continue;

                var serviceInterId = serviceInterface.Name.GetHashCode();

                serverInstances.Add(serviceInterId, serverInstance);
            }

            foreach (var instance in serverInstances)
                if (!MpcManager.ServerInstances.ContainsKey(instance.Key)) MpcManager.ServerInstances.Add(instance.Key, instance.Value);

            return new MpcServer(() => {

                foreach (var instanseKey in serverInstances.Keys)
                    MpcManager.ServerInstances.Remove(instanseKey);

                serverInstance.Stop();
            });
        }
    }
}
