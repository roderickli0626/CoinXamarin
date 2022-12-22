using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace mycoin.Extensions
{
    public class CustomDevice : IDevice
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Rssi { get; }

        public object NativeDevice { get; set; }

        public DeviceState State { get; set; }
        public IList<AdvertisementRecord> AdvertisementRecords { get; set; }

        public Task<IReadOnlyList<IService>> GetServicesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetServicesAsync(cancellationToken);
        }

        public Task<IService> GetServiceAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetServiceAsync(id, cancellationToken);
        }

        public Task<bool> UpdateRssiAsync()
        {
            return Task.FromResult(true);
        }

        public Task<int> RequestMtuAsync(int requestValue)
        {
            return Task.FromResult(0);
        }

        public bool UpdateConnectionInterval(ConnectionInterval interval)
        {
            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
