using System.Diagnostics;
using System.Numerics;

class ProcessHandler
{
    private static ulong _id = 0;
    public class process_t
    {
        private ulong id;
        private Process process;

        public process_t(Process process)
        {
            id = _id++;
            this.process = process;
        }
        public ulong get_id() { return id; }
        public Process get_process() { return process;}
    }
    private static List<process_t> _processes;
    static ProcessHandler(){
        _processes = new List<process_t>();
    }
    public static ulong AddProcess(Process process)
    {
        process_t process_t = new process_t(process);
        _processes.Add(process_t);
        return process_t.get_id();
    }
    public static process_t? GetProcess(ulong id)
    {
        return _processes.Find(x => x.get_id() == id);
    }

    public static List<process_t> GetProcesses() {
        return _processes;
    }

    public static bool DeleteProcess(ulong id){
        var process = GetProcess(id);
        if(process == null){
            return false;
        }
        return _processes.Remove(process);
    }
}