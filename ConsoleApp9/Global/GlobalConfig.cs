using Grpc.Net.Client;
using static GrpcService1.Todos;

namespace ConsoleApp9.Global
{
    public static class GlobalConfig
    {
        private static GrpcChannel grpcChannel;
        private static TodosClient todosClient;
        public static GrpcChannel GrpcChannel
        {
            get
            {
                if (grpcChannel == null)
                {
                    grpcChannel = GrpcChannel.ForAddress("https://localhost:5001");
                }
                return grpcChannel;
            }
        }


        public static TodosClient TodosClient
        {
            get
            {
                if(todosClient == null)
                {
                    todosClient = new TodosClient(GrpcChannel);
                }
                return todosClient;
            }
        }

    }
}
