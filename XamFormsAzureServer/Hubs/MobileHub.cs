using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using XamFormsAzureServer.Models;

namespace XamFormsAzureServer.Hubs
{
    public class MobileHub : Hub
    {
        static MobileHub()
        {
            StoredData = new ConcurrentDictionary<Guid, MobileInformation>();
        }

        private static ConcurrentDictionary<Guid, MobileInformation> StoredData { get; }

        public void ExchangeInformation(string deviceId, string message, string token)
        {
            try
            {
                Clients.All.Log($"Mensagem recebida de {deviceId} com token {token}:\n{message}");
                Guid guid = Guid.NewGuid();
                StoredData.TryAdd(guid, new MobileInformation(Context.ConnectionId, deviceId, message, token));
                Clients.All.Log($"Salvando item {guid}");
                var toDelete = new List<Guid>();
                foreach (var data in GetMatchingData(deviceId, token))
                {
                    MobileInformation info = data.Value;
                    // Envia informação dos outros devices para o atual
                    Clients.All.Log($"Enviando mensagem de {info.DeviceId} para {deviceId} com token {token}");
                    Clients.Caller.OnInformationReceived(info.DeviceId, info.Message);

                    // Envia informação atual para os outros devices
                    Clients.All.Log($"Enviando mensagem de {deviceId} para {info.DeviceId}");
                    Clients.Client(info.ConnectionId).OnInformationReceived(deviceId, message);

                    toDelete.Add(data.Key);
                    toDelete.Add(guid);
                }

                foreach (Guid key in toDelete)
                {
                    MobileInformation info;
                    StoredData.TryRemove(key, out info);
                    Clients.All.Log($"Removendo item: Chave {key} - {info?.DeviceId}");
                    Clients.All.Log($"Sucesso {info != null}");
                }
            }
            catch (Exception ex)
            {
                Clients.All.Log($"Ocorreu um erro {ex}");
            }

        }

        private IEnumerable<KeyValuePair<Guid, MobileInformation>> GetMatchingData(string deviceId, string token)
        {
            return StoredData.Where(s => !s.Value.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase)
                                    && s.Value.Token.Equals(token, StringComparison.OrdinalIgnoreCase));
        }
    }
}