using System;
using System.Windows;
using System.Windows.Controls;
using Confluent.Kafka;

namespace KafkaProducerTest
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /* Reading App Configuration Values */
        string bootstrapServers = Properties.Settings.Default.BootstrapServers;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowJSONMessage(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                textBlock4.Text = message;
            });
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Kafka Producer
            var conf = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
                //SecurityProtocol = SecurityProtocolType.Plaintext,
                //SaslMechanism = SaslMechanismType.Plain,
                //SaslPassword = "90ZRaduaUyRfNbzIJnXVIRlmkbTgfFn8",
                //SaslUsername = "y6bxsy8c"

            };

            Action<DeliveryReportResult<Null, string>> handler = r =>
            ShowJSONMessage(!r.Error.IsError
                ? $"Delivered message {r.Message.Value} to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");
            
            var p = new Producer<Null, string>(conf);
            string jsonmessage = "";
            string extraSpaces = null;
            string operationType = "write_request"; // By default we will test a write_request
            if (!String.IsNullOrEmpty(textBlock6.Text)) // extraSpaces
            {                
                string[] spaces= textBlock6.Text.Replace(" ", string.Empty).Split(',');
                
                foreach (string space in spaces)
                {
                    extraSpaces += "\"" + space + "\",";
                }
            }

            if (textBlock10.Text == "write_request") // Write request
            {
                if (textBlock2.Text != "")
                { // room
                    jsonmessage = "{\"code\":\"write_request\",\"payload\":{\"accessType\":\"" + textBlock5.Text + "\",\"deviceId\":\"" + textBlock9.Text + "\",\"accessId\":\"" + textBlock1.Text + "\",\"addressType\":null,\"checkoutHours\":" + (!String.IsNullOrEmpty(textBlock8.Text) ? "\"" + textBlock8.Text + "\"" : "null") + ",\"extraSpaces\":" + (extraSpaces != null ? "[" + extraSpaces.TrimEnd(',') + "]" : "null") + ",\"groups\":" + (!String.IsNullOrEmpty(textBlock7.Text) ? "[\"" + textBlock7.Text + "\"]" : "null") + ",\"hotelName\":null,\"id\":\"" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "\",\"mac\":null,\"room\":" + textBlock2.Text + ",\"zone\":null}}";
                }
                else
                { // zone
                    jsonmessage = "{\"code\":\"write_request\",\"payload\":{\"accessType\":\"" + textBlock5.Text + "\",\"deviceId\":\"" + textBlock9.Text + "\",\"accessId\":\"" + textBlock1.Text + "\",\"addressType\":null,\"checkoutHours\":" + (!String.IsNullOrEmpty(textBlock8.Text) ? "\"" + textBlock8.Text + "\"" : "null") + ",\"extraSpaces\":" + (extraSpaces != null ? "[" + extraSpaces.TrimEnd(',') + "]" : "null") + ",\"groups\":" + (!String.IsNullOrEmpty(textBlock7.Text) ? "[\"" + textBlock7.Text + "\"]" : "null") + ",\"hotelName\":null,\"id\":\"" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "\",\"mac\":null,\"room\":null,\"zone\":\"" + textBlock3.Text + "\"}}";
                }
            } else // Read request
            {
                operationType = "read_request";
                jsonmessage = "{\"code\":\"read_request\",\"payload\":{\"accessType\":\"" + textBlock5.Text + "\",\"deviceId\":\"" + textBlock9.Text + "\",\"accessId\":\"" + textBlock1.Text + "\",\"addressType\":null,\"checkoutHours\":" + (!String.IsNullOrEmpty(textBlock8.Text) ? "\"" + textBlock8.Text + "\"" : "null") + ",\"extraSpaces\":" + (extraSpaces != null ? "[" + extraSpaces.TrimEnd(',') + "]" : "null") + ",\"groups\":" + (!String.IsNullOrEmpty(textBlock7.Text) ? "[\"" + textBlock7.Text + "\"]" : "null") + ",\"hotelName\":null,\"id\":\"" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "\",\"mac\":null,\"room\":null,\"zone\":null}}";
            }
            p.BeginProduce(operationType, new Message<Null, string>
            {
                Value = jsonmessage
            }, handler);
            p.Flush(TimeSpan.FromSeconds(1)); // wait for up to 1 second for any inflight messages to be delivered.
        }

        private void textBlock1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock3_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock4_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock5_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock6_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock7_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock8_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock9_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBlock10_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
