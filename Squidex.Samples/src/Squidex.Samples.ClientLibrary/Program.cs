using Newtonsoft.Json;
using Squidex.ClientLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Squidex.Samples.ClientLibrary
{
    class Program
    {
        #region ClientManager

        public static readonly SquidexClientManager ClientManager = new SquidexClientManager(
            "https://cloud.squidex.io/",
            "clientmanager",
            "clientmanager:default",
            "ekC78jTTWq9XopmV07SLfGeHi66qQtiJWS2r9AgnU98=");

        #endregion

        #region NotificationInterfaceEntity
        public sealed class NotificationEntity : SquidexEntityBase<NotificationEntityData>
        {
        }
        #endregion

        #region NotificationInterfaceData

        public sealed class NotificationEntityData
        {
            [JsonConverter(typeof(InvariantConverter))]
            [JsonProperty("content")]
            public string Content { get; set; }
        }

        #endregion

        #region NotificationInterface

        public interface INotificationInterface
        {
            Task<List<NotificationEntity>> GetAsync();
            Task<NotificationEntity> GetAsync(string id);
            Task<(long Total, List<NotificationEntity> Datas)> GetAsync(long? skip = null, long? top = null, string filter = null, string orderBy = null, string search = null, QueryContext context = null);
            Task PostAsync(NotificationEntityData data, bool publish);
            Task UpdateAsync(string id, NotificationEntityData newData);
            Task DeleteAsync(string id);
            Task PublishAsync(string id);
            Task UnpublishAsync(string id);
        }

        #endregion

        #region NotificationClient

        public class NotificationClient : INotificationInterface
        {
            private readonly SquidexClient<NotificationEntity, NotificationEntityData> _client;
            public NotificationClient()
            {
                _client = ClientManager.GetClient<NotificationEntity, NotificationEntityData>("notifications");
            }
            public async Task<List<NotificationEntity>> GetAsync()
            {
                var notifications = await _client.GetAsync();

                return notifications.Items;
            }
            public async Task<NotificationEntity> GetAsync(string id)
            {
                var notification = await _client.GetAsync(id);

                return notification;
            }
            public async Task<(long Total, List<NotificationEntity> Datas)> GetAsync(long? skip = null, long? top = null, string filter = null, string orderBy = null, string search = null, QueryContext context = null)
            {
                var notifications = await _client.GetAsync(skip, top, filter, orderBy, search, context);

                return (notifications.Total, notifications.Items);
            }
            public async Task PostAsync(NotificationEntityData data, bool publish = false) => await _client.CreateAsync(data, publish);
            public async Task UpdateAsync(string id, NotificationEntityData newData) => await _client.UpdateAsync(id, newData);
            public async Task DeleteAsync(string id) => await _client.DeleteAsync(id);
            public async Task PublishAsync(string id) => await _client.PublishAsync(id);
            public async Task UnpublishAsync(string id) => await _client.UnpublishAsync(id);
            public async Task ArchiveAsync(string id) => await _client.ArchiveAsync(id);
        }

        #endregion

        static async Task Main(string[] args)
        {
            #region Randon Number

            Random random = new Random();
            var number = random.Next(0, 10000);

            #endregion

            Console.WriteLine("Samples - Init");

            #region PostAsync

            try
            {
                Console.WriteLine("PostAsync - Init");

                var _client = new NotificationClient();

                await _client.PostAsync(new NotificationEntityData() { Content = $"Hello World {number}" }, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PostAsync - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("PostAsync - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion

            #region GetAsync

            try
            {
                Console.WriteLine("GetAsync - Init");

                var _client = new NotificationClient();

                var notifications = await _client.GetAsync();

                foreach (var item in notifications)
                {
                    Console.WriteLine($"ID:{item.Id} - Content:{item.Data.Content} Created:{item.Created }");
                }

                Console.WriteLine($"GetAsync - Total:{notifications.Count} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAsync - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("GetAsync - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion

            #region GetAsyncById

            try
            {
                Console.WriteLine("GetAsyncById - Init");

                var _client = new NotificationClient();

                var notifications = await _client.GetAsync();

                Console.WriteLine($"GetAsync - Total:{notifications.Count} ");

                var notification = await _client.GetAsync(notifications[0].Id);

                Console.WriteLine($"GetAsync  By ID:{notifications[0].Id}");

                Console.WriteLine($"ID:{notification.Id} - Content:{notification.Data.Content} Created:{notification.Created }");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAsyncById - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("GetAsyncById - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion

            #region GetAsync Filter Skip

            try
            {
                Console.WriteLine("GetAsync Skip - Init");

                var _client = new NotificationClient();

                var (total, notifications) = await _client.GetAsync(skip: 5);

                Console.WriteLine($"GetAsync - Total:{total} ");

                Console.WriteLine($"GetAsync - Selected:{notifications.Count} ");

                foreach (var item in notifications)
                {
                    Console.WriteLine($"ID:{item.Id} - Content:{item.Data.Content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAsync - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("GetAsync Skip - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion

            #region UpdateAsync

            try
            {
                Console.WriteLine("UpdateAsync - Init");

                var _client = new NotificationClient();
                var (oldTotal, oldNotification) = await _client.GetAsync(top: 1);

                Console.WriteLine("\nOld Values");

                Console.WriteLine($"ID:{oldNotification[0].Id} - Content:{oldNotification[0].Data.Content} Created:{oldNotification[0].Created }");

                await _client.UpdateAsync(oldNotification[0].Id, new NotificationEntityData() { Content = $"Hello World {number + 1}" });

                var newNotifications = await _client.GetAsync(oldNotification[0].Id);

                Console.WriteLine("\nNew Values");

                Console.WriteLine($"ID:{oldNotification[0].Id} - Content:{newNotifications.Data.Content} Created:{newNotifications.Created } \n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateAsync - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("UpdateAsync - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion

            //#region DeleteAsync

            //try
            //{
            //    Console.WriteLine("DeleteAsync - Init");

            //    var _client = new NotificationClient();

            //    var (total, notification) = await _client.GetAsync(top: 1);

            //    Console.WriteLine($"DeleteAsync - Total:{total} ");

            //    Console.WriteLine($"DeleteAsync - Selected:{notification.Count} ");

            //    Console.WriteLine($"ID:{notification[0].Id} - Content:{notification[0].Data.Content} Created:{notification[0].Created }");

            //    await _client.DeleteAsync(notification[0].Id);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"DeleteAsync - Error | {ex.Message}");
            //}
            //finally
            //{
            //    Console.WriteLine("DeleteAsync - End");
            //    Console.WriteLine("\n-----------------------------------------------------------\n");
            //}

            //#endregion

            #region PublishAsync

            try
            {
                Console.WriteLine("PublishAsync - Init");

                Console.WriteLine("PublishAsync - Post Unpublish");

                var _client = new NotificationClient();

                await _client.PostAsync(new NotificationEntityData() { Content = $"Hello World {number}" });

                Console.WriteLine("PostAsync - Unpublish:OK");

                //Get Unpublish Itens
                var ctx = QueryContext.Default.Unpublished(true);
                var (total, notifications) = await _client.GetAsync(filter: "status eq 'Draft'", context: ctx);

                foreach (var item in notifications)
                {
                    await _client.PublishAsync(item.Id);

                    Console.WriteLine($"PostAsync - Publish:{item.Id} - OK");
                }

                Console.WriteLine($"PublishAsync - Total Publish:{notifications.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PublishAsync - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("PublishAsync - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion

            #region UnpublishAsync

            try
            {
                Console.WriteLine("UnpublishAsync - Init");

                Console.WriteLine("UnpublishAsync - Get Unpublish");

                var _client = new NotificationClient();
                var (oldTotal, notifications) = await _client.GetAsync(top: 2);

                foreach (var item in notifications)
                {
                    await _client.UnpublishAsync(item.Id);

                    Console.WriteLine($"UnpublishAsync - Unpublish Item:{item.Id} - OK");
                }

                Console.WriteLine($"UnpublishAsync - Total Unpublish:{notifications.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UnpublishAsync - Error | {ex.Message}");
            }
            finally
            {
                Console.WriteLine("UnpublishAsync - End");
                Console.WriteLine("\n-----------------------------------------------------------\n");
            }

            #endregion


            Console.WriteLine("Samples - End");
            Console.ReadKey();
        }
    }
}
