using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Sample : WebServer
{
    public override string Respond(HttpListenerRequest request)
    {
        string name = request.Url.LocalPath;
        string[] args = new string[] { };

        if (name.Contains(":"))
        {
            name = request.Url.LocalPath.Split(':')[0];
            args = request.Url.LocalPath.Split(':')[1].Split('&');
        }

        Debug.Log("Path: " + name);
        Debug.Log("Arguments: " + System.String.Join(", ", args));

        switch (name)
        {
            case "/hello":
                return "Hello World!";

            case "/getpos":
                return GetPosition(args[0]);

            default:
                return string.Format("<!DOCTYPE html> <html> <head> <title>Unity Webserver</title> </head> <body> <h1>Hello from your Unity project</h1> <p>Have fun!</p> </body> </html>");
        }
    }

    public string GetPosition(string targetName)
    {
        if (GameObject.Find(targetName) != null)
            return (GameObject.Find(targetName).transform.position.ToString());
        else
            Debug.LogWarning("Target object not found");
        return ("Target object not found");
    }
}
