using System;
using System.Xml;
using System.Xml.Serialization;

namespace LEA_for_ED
{
    class Program
    {
        static Settings settings;

        static void Main(string[] args)
        {
            // Deserialize Settings.xml to Settings object.
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            settings = (Settings)serializer.Deserialize(new XmlTextReader("Settings.xml"));

            // Setup ED Status Reader
            StatusReader.Start(settings);
            StatusReader.StatusChanged += new EventHandler<StatusChangedEventArgs>(StatusChanged);

            // Connect to LEA Server and start communications
            LeaConnection.Start(settings);

            // Shutdown
            LeaConnection.Stop();
            StatusReader.Stop();
        }

        static void StatusChanged(object obj, StatusChangedEventArgs e)
        {
            LeaConnection.SendToClient(e.PropertyName.ToString(), Convert.ChangeType(e.Value, e.PropertyType).ToString());
            //Console.WriteLine(e.TimeStamp.ToString() + " : " + e.PropertyName.ToString() + " = " + Convert.ChangeType(e.Value, e.PropertyType).ToString());
        }
    }
}
