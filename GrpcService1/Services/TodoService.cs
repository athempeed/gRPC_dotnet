using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace GrpcService1
{
    public class TodoService:Todos.TodosBase
    {
        private readonly ILogger<TodoService> _logger;
        public TodoService(ILogger<TodoService> logger)
        {
            _logger = logger;
        }
        public static List<TodoModel> _list = new List<TodoModel>();
        //{
        //      new TodoModel{ ID=1,  Name="test", CreatedDate= new Timestamp(), Status=false},
        //        new TodoModel{ ID=2,Name="test1", CreatedDate= new Timestamp(), Status=false},
        //        new TodoModel{ ID=3,Name="test2", CreatedDate= new Timestamp(), Status=false},
        //        new TodoModel{ ID=4,Name="test3", CreatedDate= new Timestamp(), Status=true},
        //        new TodoModel{ ID=5,Name="test4", CreatedDate= new Timestamp(), Status=true}
        //};
        
        //public List<TodoModel> List()
        ////{
        //    return _list;    
        //}


        public override Task<TodoList> GetTodos(EmptyRequest request, ServerCallContext context)
        {
            return Task.Run(() => {
                var list = GetList();
                var returnList = new TodoList()
                {
                    List = { list }
                };
                return returnList;
            });
        }

        public override Task<RPCResponse> GetTodoByID(TodoModel request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var response = new RPCResponse();
                if (request != null)
                {
                    var model = GetList().FirstOrDefault(x => x.ID == request.ID);
                    if(model != null)
                    {
                        response.IsDone = true;
                        response.Model = model;
                        
                    }                    
                }
                return  response;
            });
            
        }

        public override Task<RPCResponse> GetTodoByName(TodoModel request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var response = new RPCResponse();
                if (request != null)
                {
                    var model = GetList().FirstOrDefault(x => x.Name == request.Name);
                    if (model != null)
                    {
                        response.IsDone = true;
                        response.Model = model;

                    }
                }
                return response;
            });
        }


        public override Task<RPCResponse> InsertTodo(TodoRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request != null)
                {
                     _list.Add(request.Model);
                    return new RPCResponse {  IsDone = true};
                }
                return new RPCResponse { IsDone = false };
            });

        }

        public override Task<RPCResponse> updateTodo(TodoRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request != null)
                {
                    var todo = _list.FirstOrDefault(x => x.ID == request.Model.ID);
                    if(todo != null)
                    {
                        _list.Remove(todo);
                        todo = request.Model;
                        _list.Add(request.Model);
                    }
                    return new RPCResponse { IsDone = true };
                }
                return new RPCResponse { IsDone = false };
            });
        }


        public override Task<RPCResponse> DeleteTodo(TodoRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request != null)
                {
                    var todo = _list.FirstOrDefault(x => x.ID == request.Model.ID);
                    if (todo != null)
                    {
                        _list.Remove(todo);                        
                    }
                    return new RPCResponse { IsDone = true };
                }
                return new RPCResponse { IsDone = false };
            });
        }

        //public

        public List<TodoModel> GetList()
        {
            return _list;
        }


    }
}
