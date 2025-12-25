using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace PingerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static readonly HttpClient HttpClient = new HttpClient();
    private bool _isMapInitialized = false;

    private List<string> _pendingMapScripts = new List<string>();

    public MainWindow()
    {
        InitializeComponent();
        InitializeMapAsync();
    }

    /// <summary>
    /// Initialisiert das WebView2 Control und lädt die Leaflet-Karte.
    /// </summary>
    private async void InitializeMapAsync()
    {
        try
        {
            await MapWebView.EnsureCoreWebView2Async();
            
            MapWebView.NavigationCompleted += async (s, e) =>
            {
                _isMapInitialized = true;
                foreach (var script in _pendingMapScripts)
                {
                    await MapWebView.ExecuteScriptAsync(script);
                }
                _pendingMapScripts.Clear();
            };

            string htmlTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <title>Leaflet Map</title>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"" />
    <script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
    <style>
        body { margin: 0; padding: 0; }
        #map { height: 100vh; width: 100vw; background: #aad3df; }
    </style>
</head>
<body>
    <div id=""map""></div>
    <script>
        var map = L.map('map').setView([20, 0], 2);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap'
        }).addTo(map);

        var markers = [];
        var path = L.polyline([], {color: 'red'}).addTo(map);

        function clearMap() {
            markers.forEach(m => map.removeLayer(m));
            markers = [];
            path.setLatLngs([]);
            map.setView([20, 0], 2);
        }

        function addHop(lat, lon, label) {
            var marker = L.marker([lat, lon]).addTo(map).bindPopup(label);
            markers.push(marker);
            path.addLatLng([lat, lon]);
            
            // Fokus auf den Pfad anpassen
            if (markers.length > 1) {
                map.fitBounds(path.getBounds(), { padding: [50, 50] });
            } else {
                map.setView([lat, lon], 4);
            }
        }
    </script>
</body>
</html>";
            MapWebView.NavigateToString(htmlTemplate);
            _isMapInitialized = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Initialisieren der Karte: {ex.Message}");
        }
    }

    /// <summary>
    /// Repräsentiert Geo-Informationen zu einer IP-Adresse.
    /// </summary>
    public class GeoInfo
    {
        public string status { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string isp { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
    }

    /// <summary>
    /// Erweitertes Ergebnis für einen Traceroute-Hop.
    /// </summary>
    public class HopInfo
    {
        public int Hop { get; set; }
        public string Address { get; set; }
        public string Time { get; set; }
        public string NetTime { get; set; }
        public string Geo { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }

    /// <summary>
    /// Ruft Geo-Informationen für eine IP-Adresse ab.
    /// Nutzt den kostenlosen Dienst ip-api.com.
    /// </summary>
    private async Task<GeoInfo> GetGeoLocationDataAsync(string ip)
    {
        if (string.IsNullOrEmpty(ip) || ip == "0.0.0.0" || ip == "::1")
        {
            return new GeoInfo { status = "fail", city = "Lokaler Host" };
        }

        // Einfache Prüfung auf private IPv4 und IPv6 Adressbereiche
        if (ip.StartsWith("10.") || ip.StartsWith("192.168.") || ip.StartsWith("172.") || 
            ip.StartsWith("fe80:") || ip.StartsWith("fd"))
        {
            return new GeoInfo { status = "fail", city = "Lokales Netzwerk" };
        }

        try
        {
            string response = await HttpClient.GetStringAsync($"http://ip-api.com/json/{ip}");
            return JsonSerializer.Deserialize<GeoInfo>(response);
        }
        catch
        {
            return null;
        }
    }

    private async Task ExecuteMapScriptAsync(string script)
    {
        if (_isMapInitialized)
        {
            await MapWebView.ExecuteScriptAsync(script);
        }
        else
        {
            _pendingMapScripts.Add(script);
        }
    }

    /// <summary>
    /// Event-Handler für den Ping-Button.
    /// Führt einen einfachen asynchronen Ping an die angegebene Adresse aus.
    /// </summary>
    private async void PingButton_Click(object sender, RoutedEventArgs e)
    {
        string address = AddressTextBox.Text.Trim();
        if (string.IsNullOrEmpty(address))
        {
            MessageBox.Show("Bitte geben Sie eine Adresse ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        PingButton.IsEnabled = false;
        ResultTextBox.AppendText($"Pinge {address}...\n");

        try
        {
            using Ping pinger = new Ping();
            // Erwärmen des Pingers
            await pinger.SendPingAsync("127.0.0.1", 100);

            Stopwatch sw = new Stopwatch();
            // Sende Ping asynchron
            sw.Start();
            PingReply reply = await pinger.SendPingAsync(address);
            sw.Stop();

            if (reply.Status == IPStatus.Success)
            {
                // Nutze Stopwatch für höhere Präzision bei sub-ms Werten. 
                // PingReply.RoundtripTime ist ein Long-Wert in Millisekunden.
                double elapsedMs = sw.Elapsed.TotalMilliseconds;
                ResultTextBox.AppendText($"Antwort von {reply.Address}: Zeit={elapsedMs:F2}ms (Roundtrip={reply.RoundtripTime}ms)\n");
            }
            else
            {
                ResultTextBox.AppendText($"Ping fehlgeschlagen: {reply.Status}\n");
            }
        }
        catch (Exception ex)
        {
            ResultTextBox.AppendText($"Fehler: {ex.Message}\n");
        }
        finally
        {
            PingButton.IsEnabled = true;
            ResultTextBox.ScrollToEnd();
        }
    }

    /// <summary>
    /// Event-Handler für den Scan-Button.
    /// Scannt einen Bereich von IP-Adressen (IPv4) parallel.
    /// </summary>
    private async void ScanButton_Click(object sender, RoutedEventArgs e)
    {
        string baseIp = ScannerBaseIpTextBox.Text.Trim();
        if (!int.TryParse(ScannerStartTextBox.Text, out int start) || !int.TryParse(ScannerEndTextBox.Text, out int end))
        {
            MessageBox.Show("Bitte geben Sie gültige Start- und Endwerte ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ScanButton.IsEnabled = false;
        ScannerResultTextBox.Clear();
        ScannerResultTextBox.AppendText($"Scanne {baseIp}.{start} bis {baseIp}.{end}...\n");

        var tasks = new List<Task<PingReply>>();

        // Erstelle Ping-Tasks für jede IP im Bereich
        for (int i = start; i <= end; i++)
        {
            string ip = $"{baseIp}.{i}";
            tasks.Add(PingHostAsync(ip));
        }

        try
        {
            // Warte auf alle Ping-Antworten parallel
            var results = await Task.WhenAll(tasks);
            int foundCount = 0;
            foreach (var reply in results)
            {
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    ScannerResultTextBox.AppendText($"{reply.Address} ist erreichbar.\n");
                    foundCount++;
                }
            }
            ScannerResultTextBox.AppendText($"Scan abgeschlossen. {foundCount} Geräte gefunden.\n");
        }
        catch (Exception ex)
        {
            ScannerResultTextBox.AppendText($"Fehler beim Scan: {ex.Message}\n");
        }
        finally
        {
            ScanButton.IsEnabled = true;
            ScannerResultTextBox.ScrollToEnd();
        }
    }

    /// <summary>
    /// Hilfsmethode zum asynchronen Pingen eines einzelnen Hosts mit Timeout.
    /// </summary>
    private async Task<PingReply> PingHostAsync(string ip)
    {
        try
        {
            using Ping pinger = new Ping();
            return await pinger.SendPingAsync(ip, 500); // Kurzer Timeout (500ms) für schnellen Scan
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Event-Handler für den Traceroute-Button.
    /// Verfolgt den Weg der Pakete zum Ziel durch schrittweise Erhöhung der TTL.
    /// </summary>
    private async void TracerouteButton_Click(object sender, RoutedEventArgs e)
    {
        string target = TracerouteTargetTextBox.Text.Trim();
        if (string.IsNullOrEmpty(target)) return;

        TracerouteButton.IsEnabled = false;
        TracerouteResultTextBox.Clear();
        TracerouteResultTextBox.AppendText($"Traceroute zu {target}...\n");
        TracerouteVisualList.Items.Clear();

        await ExecuteMapScriptAsync("clearMap();");

        try
        {
            const int maxHops = 30;
            const int timeout = 1000;
            byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            using Ping pinger = new Ping();
            // Erwärmen
            await pinger.SendPingAsync("127.0.0.1", 100);

            for (int ttl = 1; ttl <= maxHops; ttl++)
            {
                PingOptions options = new PingOptions(ttl, true);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                PingReply reply = await pinger.SendPingAsync(target, timeout, buffer, options);
                sw.Stop();

                if (reply.Status == IPStatus.Success || reply.Status == IPStatus.TtlExpired)
                {
                    var geoData = await GetGeoLocationDataAsync(reply.Address.ToString());
                    string geoString = geoData.status == "success" 
                        ? $"{geoData.country}, {geoData.city} ({geoData.isp})" 
                        : geoData.city ?? "Unbekannter Ort";

                    double elapsedMs = sw.Elapsed.TotalMilliseconds;
                    var hop = new HopInfo
                    {
                        Hop = ttl,
                        Address = reply.Address.ToString(),
                        Time = $"{elapsedMs:F2} ms",
                        NetTime = $"{reply.RoundtripTime} ms",
                        Geo = geoString,
                        Lat = geoData.status == "success" ? geoData.lat : (double?)null,
                        Lon = geoData.status == "success" ? geoData.lon : (double?)null
                    };

                    TracerouteVisualList.Items.Add(hop);
                    TracerouteResultTextBox.AppendText($"{ttl}\t{reply.Address}\tLokal: {elapsedMs:F2} ms (Netz: {reply.RoundtripTime} ms)\t{geoString}\n");

                    if (hop.Lat.HasValue && hop.Lon.HasValue)
                    {
                        string label = $"Hop {hop.Hop}: {hop.Address}<br>{hop.Geo}";
                        await ExecuteMapScriptAsync($"addHop({hop.Lat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {hop.Lon.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}, '{label.Replace("'", "\\'")}');");
                    }
                }
                else if (reply.Status == IPStatus.TimedOut)
                {
                    double elapsedMs = sw.Elapsed.TotalMilliseconds;
                    TracerouteVisualList.Items.Add(new HopInfo { Hop = ttl, Address = "*", Time = $"{elapsedMs:F2} ms", NetTime = "Timeout", Geo = "-" });
                    TracerouteResultTextBox.AppendText($"{ttl}\t*\tLokal: {elapsedMs:F2} ms (Netz: Timeout)\n");
                }
                else
                {
                    double elapsedMs = sw.Elapsed.TotalMilliseconds;
                    TracerouteVisualList.Items.Add(new HopInfo { Hop = ttl, Address = "Fehler", Time = $"{elapsedMs:F2} ms", NetTime = reply.Status.ToString(), Geo = reply.Status.ToString() });
                    TracerouteResultTextBox.AppendText($"{ttl}\tFehler: {reply.Status} (Lokal: {elapsedMs:F2} ms)\n");
                    break;
                }

                if (reply.Status == IPStatus.Success)
                {
                    TracerouteResultTextBox.AppendText("Ziel erreicht.\n");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            TracerouteResultTextBox.AppendText($"Fehler: {ex.Message}\n");
        }
        finally
        {
            TracerouteButton.IsEnabled = true;
            TracerouteResultTextBox.ScrollToEnd();
        }
    }
}