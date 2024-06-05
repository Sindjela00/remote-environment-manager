using System.Diagnostics;
using System.Numerics;

class ProcessHandler
{
    public struct process_t
    {
        public ulong id;
        public Process process;

        public process_t(ulong id, Process process)
        {
            this.id = id;
            this.process = process;
        }
    }
    private ulong id;
    private List<process_t> processes;
    public ProcessHandler()
    {
        processes = new List<process_t>();
    }
    public ulong AddProcess(Process process)
    {
        process_t process_t = new process_t(id,process);
        processes.Add(process_t);
        return id++;
    }
    public process_t GetProcess(ulong id)
    {
        return processes.Find(x => x.id == id);
    }
}