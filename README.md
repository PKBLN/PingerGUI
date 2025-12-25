# PingerWPF

[English](#english) | [Deutsch](#deutsch)

---

<a name="english"></a>
## English

PingerWPF is a lightweight Windows desktop application built with WPF and .NET 10. It provides essential networking tools like Ping, IP Scanning, and Traceroute with a visual map representation of the network hops.

### Features

- **Ping**: Quickly check the availability and response time of a specific host or IP address.
- **IP Scanner**: Scan a range of IP addresses in a local network to identify active devices.
- **Traceroute**: Trace the path of packets to a destination, showing each hop along the way.
- **Visual Map**: Visualize Traceroute hops on an interactive world map using Leaflet and Geo-IP data.
- **Geo-IP Integration**: Automatically retrieve location and ISP information for network hops.

### Screenshots

*(Add screenshots here)*

### Technologies Used

- **Framework**: .NET 10.0 (Windows)
- **UI**: WPF (Windows Presentation Foundation)
- **Map Visualization**: [Leaflet](https://leafletjs.com/) via [Microsoft.Web.WebView2](https://learn.microsoft.com/en-us/microsoft-edge/webview2/)
- **Data Source**: OpenStreetMap, IP-API (for Geo-IP data)

### Getting Started

#### Prerequisites

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Microsoft Edge WebView2 Runtime

#### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/PingerWPF.git
   ```
2. Navigate to the project directory:
   ```bash
   cd PingerWPF
   ```
3. Build and run the application:
   ```bash
   dotnet run --project PingerWPF/PingerWPF.csproj
   ```

### Usage

1. **Ping**: Enter a hostname or IP and click "Ping".
2. **IP-Scanner**: Enter the base IP (e.g., 192.168.1) and the range (Start/End), then click "Scan".
3. **Traceroute**: Enter the target and click "Trace". The results will appear in the log, the hop list, and the "Weltkarte" (World Map) tab.

### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### Acknowledgments

- [Leaflet](https://leafletjs.com/) for the interactive maps.
- [OpenStreetMap](https://www.openstreetmap.org/) for map data.
- [ip-api.com](https://ip-api.com/) for the free Geo-IP service.

---

<a name="deutsch"></a>
## Deutsch

PingerWPF ist eine leichtgewichtige Windows-Desktop-Anwendung, die mit WPF und .NET 10 entwickelt wurde. Sie bietet grundlegende Netzwerk-Tools wie Ping, IP-Scanning und Traceroute mit einer visuellen Kartendarstellung der Netzwerk-Hops.

### Funktionen

- **Ping**: Überprüfen Sie schnell die Verfügbarkeit und Antwortzeit eines bestimmten Hosts oder einer IP-Adresse.
- **IP-Scanner**: Scannen Sie einen Bereich von IP-Adressen in einem lokalen Netzwerk, um aktive Geräte zu identifizieren.
- **Traceroute**: Verfolgen Sie den Pfad von Paketen zu einem Ziel und zeigen Sie jeden Hop auf dem Weg an.
- **Visuelle Karte**: Visualisieren Sie Traceroute-Hops auf einer interaktiven Weltkarte mit Leaflet und Geo-IP-Daten.
- **Geo-IP-Integration**: Automatische Abfrage von Standort- und ISP-Informationen für Netzwerk-Hops.

### Screenshots

*(Screenshots hier einfügen)*

### Verwendete Technologien

- **Framework**: .NET 10.0 (Windows)
- **UI**: WPF (Windows Presentation Foundation)
- **Kartenvisualisierung**: [Leaflet](https://leafletjs.com/) via [Microsoft.Web.WebView2](https://learn.microsoft.com/en-us/microsoft-edge/webview2/)
- **Datenquelle**: OpenStreetMap, IP-API (für Geo-IP Daten)

### Erste Schritte

#### Voraussetzungen

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Microsoft Edge WebView2 Runtime

#### Installation

1. Klonen Sie das Repository:
   ```bash
   git clone https://github.com/yourusername/PingerWPF.git
   ```
2. Navigieren Sie in das Projektverzeichnis:
   ```bash
   cd PingerWPF
   ```
3. Erstellen und starten Sie die Anwendung:
   ```bash
   dotnet run --project PingerWPF/PingerWPF.csproj
   ```

### Bedienung

1. **Ping**: Geben Sie einen Hostnamen oder eine IP ein und klicken Sie auf "Ping".
2. **IP-Scanner**: Geben Sie die Basis-IP (z.B. 192.168.1) und den Bereich (Start/Ende) ein, und klicken Sie auf "Scan".
3. **Traceroute**: Geben Sie das Ziel ein und klicken Sie auf "Trace". Die Ergebnisse erscheinen im Log, in der Hop-Liste und im Reiter "Weltkarte".

### Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert – siehe die [LICENSE](LICENSE)-Datei für Details.

### Danksagungen

- [Leaflet](https://leafletjs.com/) für die interaktiven Karten.
- [OpenStreetMap](https://www.openstreetmap.org/) für Kartendaten.
- [ip-api.com](https://ip-api.com/) für den kostenlosen Geo-IP-Service.
