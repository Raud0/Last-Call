using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public GameObject loggedObject;
    public Ticker ticker;
    private AttentionImp logged;
    private bool found = false;
    private bool logStarted = false;
    private string logname;
    private List<string> topics;
    private int previousSecond = -1;

    private void Seek()
    {
        logged = loggedObject.GetComponentInChildren<AttentionImp>();
        if (logged != null) found = true;
        else return;

        Dictionary<string, Topic> topics = logged.GetTopics();
        this.topics = new List<string>();
        foreach (string topic in topics.Keys) this.topics.Add(topic);
        this.topics.Sort();
    }

    private void StartLog()
    {
        string n1 = "alberteinstein"[Random.Range(0, 14)].ToString();
        string n2 = "nielsbohr"[Random.Range(0, 9)].ToString();
        string n3 = "jameschadwick"[Random.Range(0, 13)].ToString();
        string n4 = "ottohahn"[Random.Range(0, 8)].ToString();
        string n5 = "jrobertoppenheimer"[Random.Range(0, 18)].ToString();
        string n6 = "leoszilard"[Random.Range(0, 10)].ToString();
        string n7 = "enricofermi"[Random.Range(0, 11)].ToString();
        string n8 = "mariecurie"[Random.Range(0, 10)].ToString();
        string n9 = "hansbethe"[Random.Range(0, 9)].ToString();
        string n10 = "harrydaghlian"[Random.Range(0, 13)].ToString();
        string n11 = "johnfitzgeraldkennedy"[Random.Range(0, 21)].ToString();
        string ID = n1 + n2 + n3 + n4 + n5 + n6 + n7 + n8 + n9 + n10 + n11;
        logname = "LC_log_" + ID + ".csv";
        File.Create(logname).Close();
        
        string line = "TICKER,";

        foreach (string topic in topics)
        {
            line += topic + ",";
        }
        if (line[line.Length - 1] == ',') line = line.Substring(0, line.Length - 1);
        
        StreamWriter sw = new StreamWriter(File.Open(logname, FileMode.Append));
        sw.WriteLine(line);
        sw.Close();

        logStarted = true;
    }
    
    void Update()
    {
        if (found)
        {
            if (logStarted)
            {
                DoLogging();
            }
            else
            {
                StartLog();
            }
        }
        else
        {
            Seek();
        }
    }

    private void DoLogging()
    {
        if (ticker == null) return;
        int thisSecond = ticker.GetSeconds();
        if (thisSecond == previousSecond) return;
        previousSecond = thisSecond;

        string line = thisSecond + ",";

        Dictionary<string, Topic> topicStates = logged.GetTopics();
        foreach (string topic in topics)
        {
            line += topicStates[topic].Focus + ",";
        }
        if (line[line.Length - 1] == ',') line = line.Substring(0, line.Length - 1);
        
        StreamWriter sw = new StreamWriter(File.Open(logname, FileMode.Append));
        sw.WriteLine(line);
        sw.Close();
    }
}
