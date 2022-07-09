using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyApp 
{
    internal class Program
    {

        static void Main(string[] args)
        {


          
                var connectionFactory = new ConnectionFactory
                {
                    // HostName = "localhost",
                    // UserName = "test",
                    // Password = "test",
                    // VirtualHost = "/",
                    Uri = new Uri("amqps://test:test@localhost:5671"),
                    RequestedHeartbeat = new TimeSpan(60),
                    AutomaticRecoveryEnabled = true,
                    Port = AmqpTcpEndpoint.DefaultAmqpSslPort,
                    Ssl = new SslOption(){
                        Enabled = true,
                        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors} ,

                
               
                }; 
               
              
                // connectionFactory.Ssl.ServerName = "rabbit@4329e9edb747";
                //connectionFactory.Ssl.CertPath = "client-cert.pem";
                // connectionFactory.Ssl.CertPassphrase = "random_pw";
                //connectionFactory.Ssl.Enabled = true;
                //connectionFactory.Port = 5671;

                //connectionFactory.Uri = new Uri("amqps://test:test@localhost:5671");

                using (var connection = connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        
                            channel.QueueDeclare("demo-queue",
                                durable:true,
                                exclusive:false,
                                autoDelete: false,
                                arguments: null

                            );
                        var consumer = new EventingBasicConsumer(channel);
                            consumer.Received += (sender , e) =>{
                                var body = e.Body.ToArray();
                                var message = Encoding.UTF8.GetString(body);
                                System.Console.WriteLine(message);
                            };
                        channel.BasicConsume("demo-queue",true,consumer);
                        Console.ReadLine();
                    }
                }


                //  var connection = connectionFactory.CreateConnection();
                
                //  var channel = connection.CreateModel();
                // channel.QueueDeclare("demo-queue",
                //     durable:true,
                //     exclusive:false,
                //     autoDelete: false,
                //     arguments: null

                // );

                // var consumer = new EventingBasicConsumer(channel);
                // consumer.Received += (sender , e) =>{
                //     var body = e.Body.ToArray();
                //     var message = Encoding.UTF8.GetString(body);
                //     System.Console.WriteLine(message);
                // };

                // channel.BasicConsume("demo-queue",true,consumer);
                // Console.ReadLine();
            


            //-------------


            // var factory = new ConnectionFactory{

            //     Uri = new Uri("amqps://test:test@localhost:5672/"),
                
            //     Ssl = new SslOption(){
            //         Version = SslProtocols.Tls12,
            //         Enabled = true,
            //         CertPath = @"../../../certificates3/client-cert.pem",
            //         ServerName="host.docker.internal"
                                                
                                                
            //                                     }
                

            // };
            
            // using var connection = factory.CreateConnection();
            
            // using var channel = connection.CreateModel();
            // channel.QueueDeclare("demo-queue",
            //     durable:true,
            //     exclusive:false,
            //     autoDelete: false,
            //     arguments: null

            // );

            // var consumer = new EventingBasicConsumer(channel);
            // consumer.Received += (sender , e) =>{
            //     var body = e.Body.ToArray();
            //     var message = Encoding.UTF8.GetString(body);
            //     System.Console.WriteLine(message);
            // };

            // channel.BasicConsume("demo-queue",true,consumer);
            // Console.ReadLine();


            //---------
        


        }
    }
}