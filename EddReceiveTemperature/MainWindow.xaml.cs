using System;
using System.Text;
using System.Windows;
using Microsoft.ServiceBus.Messaging;

namespace EddReceiveTemperature
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ConnectionString = "HostName=KirkSampleIotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=IqYnZ5eONOp29HOlMrMy7fqiVLgDP5Eqjh0R1/bjzeo=";
        private const string IotHubD2CEndpoint = "messages/events";
        private EventHubClient _eventHubClient;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = _eventHubClient.GetDefaultConsumerGroup()
                .CreateReceiver(partition, DateTime.UtcNow);

            while (true)
            {
                var eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                var data = Encoding.UTF8.GetString(eventData.GetBytes());

                Messages.Items.Add("Partition: " + partition + " Data: " + data);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(ConnectionString, IotHubD2CEndpoint);
            var d2CPartitions = _eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (var partition in d2CPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
        }
    }
}
