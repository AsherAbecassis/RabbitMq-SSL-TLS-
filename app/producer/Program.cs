using System;
using System.Net.Security;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            
            try
            {
                var connectionFactory = new ConnectionFactory
                {
                    // HostName = "localhost",
                    // UserName = "test",
                    // Password = "test",
                    // Port = 5671,
                    // VirtualHost = "/",
                    Uri = new Uri("amqps://test:test@localhost:5671"),
                    RequestedHeartbeat = new TimeSpan(60),
                    AutomaticRecoveryEnabled = true,
                    Ssl = new SslOption(){
                        Enabled = true,
                        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors} ,
                    
                   
                };
                connectionFactory.Ssl.CertPath = "client-cert.pem";
                // connectionFactory.Ssl.CertPassphrase = "random_pw";
                connectionFactory.Ssl.Enabled = true;
                connectionFactory.Port = 5671;
                 var connection = connectionFactory.CreateConnection();
                
                 var channel = connection.CreateModel();
                channel.QueueDeclare("demo-queue",
                    durable:true,
                    exclusive:false,
                    autoDelete: false,
                    arguments: null

                );
                

                var message = new {Name = "producer",Message= "port send amqps is -> "+connectionFactory.Port.ToString()};
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish("","demo-queue", null,body);

                
            }
            catch(Exception e)
            {
                Console.WriteLine(Environment.GetEnvironmentVariable("rabbitMqUrl"));
                var tempExcetion = e.InnerException;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                while (tempExcetion.InnerException != null)
                {
                    Console.WriteLine(tempExcetion.InnerException.Message);
                    Console.WriteLine(tempExcetion.InnerException.StackTrace);
                    tempExcetion = tempExcetion.InnerException;
                }
                // Console.WriteLine(e.StackTrace);
                throw new Exception(e.Message);
            }

            
            
            
            
            // var factory = new ConnectionFactory{

            //     Uri = new Uri("amqp://test:test@localhost:5672")

            // };
            // using var connection = factory.CreateConnection();
            // using var channel = connection.CreateModel();
            // channel.QueueDeclare("demo-queue",
            //     durable:true,
            //     exclusive:false,
            //     autoDelete: false,
            //     arguments: null

            // );
            // var message = new {Name = "producer",Message="hello2"};
            // var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            // channel.BasicPublish("","demo-queue", null,body);




        }
    }
}