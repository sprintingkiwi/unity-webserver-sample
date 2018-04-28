﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine.Events;


public class WebServer : MonoBehaviour
{
    HttpListener listener;
    public string socketAddress;
    bool ready;
    HttpListenerRequest lastRequest;
    HttpListenerContext ctx;

    [System.Serializable]
    public class ManagedRequest
    {
        [System.Serializable]
        public class MyEvent : UnityEvent<string[]> { }

        public string name;
        public MyEvent prova;
        public Func<string[], string> responseFunction;
    }
    public ManagedRequest[] managedRequests;

    public void Start()
    {
        listener = new HttpListener();

        if (!HttpListener.IsSupported)
            throw new NotSupportedException(
                "Needs Windows XP SP2, Server 2003 or later.");

        if (socketAddress == null || socketAddress.Length == 0)
            throw new ArgumentException("prefixes");

        listener.Prefixes.Add(socketAddress);

        listener.Start();

        Run();

        Debug.Log("Started server on " + socketAddress.ToString());
    }

    public void Update()
    {
        if (ready)
        {
            string rstr = Respond(ctx.Request);
            byte[] buf = Encoding.UTF8.GetBytes(rstr);
            ctx.Response.ContentLength64 = buf.Length;
            ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);

            ctx.Response.OutputStream.Close();
            ready = false;
        }
    }    

    public void Run()
    {
        ThreadPool.QueueUserWorkItem((o) =>
        {
            Debug.Log("Webserver running...");
            try
            {
                while (listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                        ctx = c as HttpListenerContext;
                        ready = true;
                        try
                        {
                            //string rstr = Respond(ctx.Request);
                            //byte[] buf = Encoding.UTF8.GetBytes(rstr);
                            //ctx.Response.ContentLength64 = buf.Length;
                            //ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                            //ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                        }
                        catch { } // suppress any exceptions
                            finally
                        {
                                // always close the stream
                                //ctx.Response.OutputStream.Close();
                        }
                    }, listener.GetContext());
                }
            }
            catch { } // suppress any exceptions
            });
    }

    public void Stop()
    {
        listener.Stop();
        listener.Close();
    }

    // You can override this method
    public virtual string Respond(HttpListenerRequest request)
    {
        string name = request.Url.LocalPath;
        string[] args = new string[] { };

        if (name.Contains(":"))
        {
            name = request.Url.LocalPath.Split(':')[0];
            args = request.Url.LocalPath.Split(':')[1].Split('&');
        }

        Debug.Log("Path: " + name);
        Debug.Log("Arguments: " + args.ToString());

        foreach (ManagedRequest mr in managedRequests)
        {
            if (mr.name == name)
                return mr.responseFunction.Invoke(args);
        }
        return string.Format("<!DOCTYPE html> <html> <head> <title>Unity Webserver</title> </head> <body> <h1>Hello from your Unity project</h1> <p>Have fun!</p> </body> </html>");
    }
}