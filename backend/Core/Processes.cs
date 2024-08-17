using System.Diagnostics;
public class process_t
{
    private static ulong _id = 0;
    private ulong id;
    private Process process;
    private string stdout;
    private string stderr;
    private int exitCode;
    private string status;


    public process_t(Process process)
    {
        id = _id++;
        this.process = process;
        this.stdout = "";
        this.stderr = "";
        this.status = "Not Finished";
    }
    public ulong get_id() { return id; }
    public string get_stdout() { return stdout; }
    public string get_stderr() { return stderr; }
    public int get_exitCode() { return exitCode; }
    public dynamic get_output() {
        return new { id,status, exitCode, stdout, stderr };
    }
    public bool Exited()
    {
        return process.HasExited;
    }
    public void parse_output()
    {
        stdout += this.process.StandardOutput.ReadToEnd();
        stderr += this.process.StandardError.ReadToEnd();
        exitCode = this.process.ExitCode;
        switch (exitCode){
            case 0:
                status = "Finished.";
                break;
            case 4:
                status = "Finished partly.";
                break;
            default:
                status = "Error.";
                break;
        }
    }
}
class Processes
{
    private List<process_t> processes = new List<process_t>();
    public ulong add_process(Process process) {
        process_t? process_t = new process_t(process);
        processes.Add(process_t);
        return process_t.get_id();
    }
    public process_t? get_process(ulong id) {
        return processes.Find(x=>x.get_id() == id);
    }
    public dynamic parse_processes() {
        List<dynamic> arr = new List<dynamic>();
        foreach (process_t process in processes) {
            if (process.Exited()) {
                process.parse_output();
                arr.Add(process.get_output());
                continue;
            }
            arr.Add(new {id=process.get_id(), status="Process not Finished."});
        }
        return arr;
    }
    private dynamic parse_output(process_t process){
        if (!process.Exited()) {
            return new {id=process.get_id(), status="Process not Finished."};
        }
        process.parse_output();
        return process.get_output();
    }
}