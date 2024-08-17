public class Machine{
    public Machine() {
        this.id = 0;
        this.name = "";
        this.hostname = "";
        this.ipv4 = "";
        this.ipv6 = "";
        this.port = 0;
        this.posx = 0;
        this.posy = 0;
        this.roomId = 0;
    }
    public Machine(int id, string name, string hostname, string ipv4, string ipv6, int port, int posx, int posy, int roomId)
    {
        this.id = id;
        this.name = name;
        this.hostname = hostname;
        this.ipv4 = ipv4;
        this.ipv6 = ipv6;
        this.port = port;
        this.posx = posx;
        this.posy = posy;
        this.roomId = roomId;
    }

    public int? id { get; set; }
    public string name { get; set; }
    public string hostname { get; set; }
    public string ipv4 { get; set; }
    public string ipv6 { get; set; }
    public int port {get; set; }
    public int posx { get; set; }
    public int posy { get; set; } 
    public int roomId { get; set; }
}