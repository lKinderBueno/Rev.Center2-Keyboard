using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;
using System.IO.Pipes;

namespace MyColor2
{
    static class MyAPPShell
    {
        static NamedPipeClientStream m_pipeClient; //connect to MyAPPShell

        static public string GetStream(string strRequest)
        {
            if (m_pipeClient.IsConnected == false)
                return null;
            try
            {
                StreamReader sr = new StreamReader(m_pipeClient);
                StreamWriter sw = new StreamWriter(m_pipeClient);

                sw.WriteLine(strRequest);
                sw.Flush();
                return sr.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine("GetStream failed {0}",e.ToString());
                return null;
            }
        }

        static public void SetComd(string strCmd)
        {
            if (m_pipeClient.IsConnected == false)
                return ;
            try
            {                
                StreamWriter sw = new StreamWriter(m_pipeClient);            
                sw.WriteLine(strCmd);
                sw.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("GetStream failed {0}", e.ToString());
            }
        }

        static public void Connect()
        {
            if (m_pipeClient == null)
            {
                try
                {
                    m_pipeClient =
                    new NamedPipeClientStream(".", "MyAPPStream", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

                    Console.WriteLine("Connecting to server...\n");
                    m_pipeClient.Connect(5000);
                    Console.WriteLine("There are currently {0} pipe server instances open.", m_pipeClient.NumberOfServerInstances);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect to MyAPPShell Failed {0}", ex.ToString());
                }

            }
        }
        static public void Disconnect()
        {
            if(m_pipeClient != null)
            {
                m_pipeClient.Close();
                m_pipeClient = null;
            }

        }

        static public void ShutDown()
        {
            SetComd("Shutdown");
            m_pipeClient = null;
        }
    }
}
