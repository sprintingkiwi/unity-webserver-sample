using System.Collections;
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
        return "";
    }
}