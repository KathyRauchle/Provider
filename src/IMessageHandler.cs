using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace MicroServiceUtilites
{
    public abstract class IMessageHandler
    {
        protected JObject Message { get; set; }
        private string Action { get; set; }

        protected IMessageHandler(Subscription subscription)
        {
            BasicDeliverEventArgs basicDeliveryEventArgs = subscription.Next();
            string incomingMessage = System.Text.Encoding.UTF8.GetString(basicDeliveryEventArgs.Body);

            try
            {
                Message = JObject.Parse(incomingMessage);
                Action = Message["action"].ToString();
                ProccessMessage();
            }
            catch (JsonException)
            {
                Console.WriteLine(new Response(ResponseCode.BAD_REQUEST, "Error processing JSON payload. please confirm message is formatted correctly.").ToString());
            }
            catch(Exception)
            {
                Console.WriteLine(new Response(ResponseCode.UNCAUGHT_EXCEPTION, "An unknown error occurred").ToString());
            }
            finally
            {
                subscription.Ack(basicDeliveryEventArgs);
            }
        }

        private void ProccessMessage()
        {
            Console.WriteLine("Message Recieved...");
            
            MethodInfo actionMethod = null;
            try
            {
                actionMethod = this.GetType().GetMethod(Action);
                Console.WriteLine((string)actionMethod.Invoke(this, null));
            }
            catch
            {
                if(actionMethod == null)
                    Console.WriteLine(new Response(ResponseCode.BAD_REQUEST,"Error calling \""+Action+"\" confirm method exists."));
                throw;
            }
        }
    }
}