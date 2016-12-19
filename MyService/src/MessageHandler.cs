using System;
using System.Linq;
using MicroServiceUtilites;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client.MessagePatterns;

namespace MyService
{
    public class MessageHandler : IMessageHandler
    {
        public MessageHandler(Subscription subscription) : base(subscription){}
        public string Add()
        {
            Provider myObject;
            try
            {
                myObject = Provider.DeserializeProvider(Message["Provider"].ToString());
                myObject.ProviderID = Guid.NewGuid();
                myObject.IsActive = myObject.IsActive;

                using (var db = new MyObjectDbContext())
                {
                    db.Provider.Add(myObject);
                    db.SaveChanges();
                }

            
                return new Response(ResponseCode.SUCCESS, myObject.ToString()).ToString();
            }
            catch (DbUpdateException e)
            {
                return MyObjectDbContext.HandleDbUpdateException(e).ToString();
            }
            catch (JsonException)
            {
                return new Response(ResponseCode.BAD_REQUEST, "Provider information not in valid format. please review JSON payload.").ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Response(ResponseCode.UNCAUGHT_EXCEPTION, "An unknown error occurred").ToString();
            }
        }

        public string Update()
        {
            Provider myObject;
            try
            {
                myObject = Provider.DeserializeProvider(Message["Provider"].ToString());
               
                using (var db = new MyObjectDbContext())
                {
                    Provider pe = db.Provider
                        .Where(b => b.ProviderID == myObject.ProviderID)
                        .SingleOrDefault();

                    pe.FirstName = myObject.FirstName;
                    pe.LastName = myObject.LastName;
                    pe.Title = myObject.Title;
                    pe.IsActive = myObject.IsActive;
                    db.SaveChanges();
                }

                return new Response(ResponseCode.SUCCESS, "Provider was updated successfully.").ToString();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                return MyObjectDbContext.HandleDbUpdateException(e).ToString();
            }
            catch (JsonException)
            {
                return new Response(ResponseCode.BAD_REQUEST, "Provider information not in valid format. please review JSON payload.").ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Response(ResponseCode.UNCAUGHT_EXCEPTION, "An unknown error occurred").ToString();
            }
        }

        public string SetIsActive()
        {
            try
            {
                Provider pe;

                Console.WriteLine(Message["providerid"].ToString());
                Console.WriteLine(Message["isactive"].ToString());
                
                Guid id = new Guid(Message["providerid"].ToString());
                bool isActive = Message["isactive"].ToObject<bool>();
                //Console.WriteLine(Message["providerid"].ToString());
                //Console.WriteLine(Message["isActive"].ToString());

                if (id == Guid.Empty)
                {
                    return new Response(ResponseCode.BAD_REQUEST, "ID must be valid BusinessEntityID. please review JSON payload.").ToString();
                }

                using (var db = new MyObjectDbContext())
                {
                    pe = db.Provider
                        .Where(b => b.ProviderID == id)
                        .SingleOrDefault();

                    pe.IsActive = isActive;
                    db.SaveChanges();
                }
                //Send event to event exchange, it's a topic exchange 
                return new Response(ResponseCode.SUCCESS,
                    "Provider was " + (isActive ? "enabled" : "disabled") + ".").ToString();
            }
            catch (DbUpdateException e)
            {
                return MyObjectDbContext.HandleDbUpdateException(e).ToString();
            }
            catch (JsonException)
            {
                return new Response(ResponseCode.BAD_REQUEST,
                    "Provider information not in valid format. please review JSON payload.").ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Response(ResponseCode.UNCAUGHT_EXCEPTION, "An unknown error occurred").ToString();
            }
        }

        public string Get()
        {
            try
            {
                Provider pe;
                Guid id = new Guid(Message["providerid"].ToString());

                if (id == Guid.Empty)
                {
                    return new Response(ResponseCode.BAD_REQUEST,
                        "ID must be valid ProviderID. please review JSON payload.").ToString();
                }

                using (var db = new MyObjectDbContext())
                {
                    pe = db.Provider
                        .Where(b => b.ProviderID == id)
                        .SingleOrDefault();

                }

                if (pe != null)
                {
                    return new Response(ResponseCode.SUCCESS, pe.ToString()).ToString();
                }
                else
                {
                    return new Response(ResponseCode.BUSINESS_ENTITY_NOT_EXIST,
                        "No BusinessEntity found matching the provided id.").ToString(); 
                }
            }
            catch (DbUpdateException e)
            {
                return MyObjectDbContext.HandleDbUpdateException(e).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Response(ResponseCode.UNCAUGHT_EXCEPTION, "An unknown error occurred").ToString();
            }
        }
    }
}