using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Sample : WebServer
{
    public Utils utils;

    public override string Respond(HttpListenerRequest request)
    {
        string path = request.Url.LocalPath;
        string query = "";

        if (path.Contains(":"))
        {
            path = request.Url.LocalPath.Split(':')[0];
            query = request.Url.LocalPath.Split(':')[1];
        }

        Debug.Log("Path: " + path);
        Debug.Log("Query: " + query);

        switch (path)
        {
            case "/hello":
                return "Hello World";

            case "/getpos":
                return utils.GetPosition(query);

            default:
                return string.Format("<!DOCTYPE html> <html> <head> <title>Unity Webserver</title> </head> <body> <h1>Hello from your Unity project</h1> <p>Have fun!</p> </body> </html>");
        }

    }
}
