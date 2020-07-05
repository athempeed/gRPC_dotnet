using ConsoleApp9.Global;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcService1;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            int choice =-1;
            TodoList reply = new TodoList();
            string name = "";
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                while(choice!=0)
                {
                    Console.WriteLine("\n\n------------------------------\n\n");
                    Console.WriteLine("1. all todos");
                    Console.WriteLine("2. Todo by ID");
                    Console.WriteLine("3. Todo by Status");
                    Console.WriteLine("4. Todo by Name");
                    Console.WriteLine("5. Create Todo");
                    Console.WriteLine("6. Update Todo");
                    Console.WriteLine("7. Delete Todo");
                    Console.WriteLine("0. Exit");
                    Console.WriteLine("\n\n------------------------------\n\n");
                    choice = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("\n\n");
                    switch (choice)
                    {
                        case 1:
                            reply  = await GetTodos();
                            if (reply != null && reply.List != null && reply.List.Count > 0)
                            {
                                reply.List.ToList().ForEach(r =>
                                {
                                    ShowTodo(r);   
                                });
                            }
                            else
                            {
                                Console.WriteLine("you do not have any todos");
                            }
                            break;
                        case 2:
                            Console.WriteLine("Enter Id to search:");
                            int id = Convert.ToInt32(Console.ReadLine());
                            var todo = await GetTodoByID(new TodoModel { ID = id });
                            if(todo != null)
                            {
                                ShowTodo(todo);
                            }
                            else
                            {
                                Console.WriteLine("you do not have Todo");
                            }
                            
                            break;
                        case 3:
                            Console.WriteLine("Enter status to search:");
                            bool status = Convert.ToBoolean(Console.ReadLine());
                            reply = await GetTodoByStatus(new TodoModel { Status = status });
                            if (reply != null && reply.List != null && reply.List.Count > 0)
                            {
                                reply.List.ToList().ForEach(r =>
                                {
                                    ShowTodo(r);
                                });
                            }
                            else
                            {
                                Console.WriteLine("you do not have Todo");
                            }
                            break;
                        case 4:
                            Console.WriteLine("Enter name to search:");
                            name = Console.ReadLine();
                            todo = await GetTodoByName(new TodoModel { Name = name });
                            if(todo != null)
                            {
                                ShowTodo(todo);
                            }
                            else
                            {
                                Console.WriteLine("you do not have todo");
                            }
                            break;
                        case 5:
                            Console.WriteLine("Enter Todo");
                             name = Console.ReadLine();
                            var result =  await InsertTodo(name);

                            if (result)
                            {
                                Console.WriteLine("Todo inseted successfully");
                            }
                            else
                            {
                                Console.WriteLine("there is some proplem. Please try again later");
                            }
                            break;
                        case 6:
                            Console.WriteLine("Search for the Toto with ID");
                            id = Convert.ToInt32(Console.ReadLine());
                            todo = await GetTodoByID(new TodoModel { ID = id });
                            if(todo != null)
                            {
                                ShowTodo(todo);
                            }
                            Console.WriteLine("Choose option to update Todo");
                            Console.WriteLine("A. Update Name");
                            Console.WriteLine("B. Update Statue");
                            char opt =Convert.ToChar(Console.ReadLine());
                            switch (opt)
                            {
                                case 'A' : case 'a':
                                    Console.WriteLine("enter name to update");
                                    var newName = Console.ReadLine();
                                    todo.Name = newName;
                                    result = await UpdateTodo(todo);
                                    break;
                                case 'B': case 'b':
                                    Console.WriteLine("enter status to update");
                                    var newStatus = Console.ReadLine();
                                    todo.Status= Convert.ToBoolean(newStatus);
                                    result = await UpdateTodo(todo);
                                    break;
                                default:
                                    result = false;
                                    Console.WriteLine("choose right option");
                                    break;

                            }
                            if (result)
                            {
                                Console.WriteLine("Todo updated successfully");
                            }
                            else
                            {
                                Console.WriteLine("there is some proplem. Please try again later");
                            }
                            break;
                        case 7:
                            Console.WriteLine("Search for the Toto with ID");
                            id = Convert.ToInt32(Console.ReadLine());
                            todo = await GetTodoByID(new TodoModel { ID = id });
                            if (todo != null)
                            {
                                ShowTodo(todo);
                            }
                            Console.WriteLine("are you sure you want to delete this Todo?press A for yes.");
                            opt = Convert.ToChar(Console.ReadLine());
                            if (opt =='A' || opt =='a') 
                            {
                                result = await DeleteTodo(todo);
                                if (result)
                                {
                                    Console.WriteLine("Todo deleted successfully");
                                }
                                else
                                {
                                    Console.WriteLine("there is some proplem. Please try again later");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Todo item not deleted");
                            }
                            break;
                        case 0:
                            Console.WriteLine("Exited....");
                            break;
                        default:
                            Console.WriteLine("Select right option....");
                            break;
                    }
                }
                

            }
            Console.ReadLine();
        }        
        public static async Task<TodoList> GetTodos()
        {
           //var channel =  GlobalConfig.GrpcChannel;
            var todos = GlobalConfig.TodosClient;
            var reply = await todos.GetTodosAsync(new EmptyRequest());
            return reply;
            
        }

        public static async Task<TodoModel> GetTodoByID(TodoModel model)
        {

            var todos = GlobalConfig.TodosClient;
            var reply = await todos.GetTodoByIDAsync(model);
            if (reply != null && reply.IsDone)
            {
                return reply.Model;
            }
            else
            {
                Console.WriteLine($"no todo found with ID :{model.ID}");
                
            }
            return null;
        }


        public static async Task<TodoModel> GetTodoByName(TodoModel model)
        {

            var todos = GlobalConfig.TodosClient;
            var reply = await todos.GetTodoByNameAsync(model);
            if (reply != null && reply.IsDone)
            {
                
                return reply.Model;
            }
            else
            {
                Console.WriteLine($"no todo found with Name :{model.Name}");
            }
            return null;
        }



        public static async Task<TodoList> GetTodoByStatus(TodoModel model)
        {
            var list = await GetTodos();
            if(list !=null && list.List !=null && list.List.Count>0)
            {
                var listy = list.List.Where(x => x.Status == model.Status);
                return new TodoList() { List = { listy } };
            }
            else
            {
                Console.WriteLine($"no todos found with status:{model.Status}");
            }


            return null;
        }


        public static void ShowTodo(TodoModel model)
        {
            Console.WriteLine($"ID: {model.ID}, Name:{model.Name}, Completed:{model.Status}");
        }

        public static async Task<bool> InsertTodo(string name)
        {
            var client = GlobalConfig.TodosClient;
            var d = new Random();
            var resp =  await client.InsertTodoAsync(new TodoRequest
            {
                Model = new TodoModel
                {
                    Name = name,
                    ID = d.Next(0,100),
                    CreatedDate = new Timestamp(),
                    Status = false
                }
            });
            return resp.IsDone;
        }

        public static async Task<bool> UpdateTodo(TodoModel model)
        {

            var client = GlobalConfig.TodosClient;
            var resp = await client.updateTodoAsync(new TodoRequest
            {
                Model = model
            });

            return resp.IsDone;
        }

        public static async Task<bool> DeleteTodo(TodoModel model)
        {

            var client = GlobalConfig.TodosClient;
            var resp = await client.DeleteTodoAsync(new TodoRequest
            {
                Model = model
            });

            return resp.IsDone;
        }

    }
}
