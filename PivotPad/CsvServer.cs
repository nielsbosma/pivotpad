using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PivotPad;

public class CsvServer
{
    private readonly string _url;

    public CsvServer(string url)
    {
        _url = url;
    }

    public async Task Serve(string csv)
    {
        var listener = new HttpListener();
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Prefixes.Add(_url);
        listener.Start();

        HttpListenerContext ctx = await listener.GetContextAsync();
        HttpListenerRequest req = ctx.Request;
        HttpListenerResponse resp = ctx.Response;

        byte[] data = Encoding.UTF8.GetBytes(csv);
        resp.ContentType = "text/csv";
        resp.ContentEncoding = Encoding.UTF8;
        resp.ContentLength64 = data.LongLength;
        resp.Headers.Add("Access-Control-Allow-Origin", "*");
        resp.Headers.Add("Access-Control-Allow-Methods", "POST, GET");

        await resp.OutputStream.WriteAsync(data, 0, data.Length);
        resp.Close();

        listener.Close();
    }
    
}