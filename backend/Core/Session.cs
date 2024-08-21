using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;

public class Session
{
    private string id;
    private string owner_id;
    private List<string> user_ids;
    private ProcessHandler handler;
    private string foldername;
    private string pulled_foldername;
    private string inventory;
    private List<Machine> machines;
    private List<KeyValuePair<string, string>> subset_inventories;
    public Session(string id , string user_id)
    {
        this.id = id;
        owner_id = user_id;
        user_ids = new List<string>();
        handler = new ProcessHandler();
        subset_inventories = new List<KeyValuePair<string, string>>();
        foldername = "";
        pulled_foldername = "";
        inventory = "";
    }
    public bool belong(string user_id){
        if (owner_id == user_id || user_ids.Contains(user_id)) {
            return true;
        }
        return false;
    }
    public bool owner(string user_id){
       return user_id == owner_id; 
    }
    public void add_permission(string user_id) {
        user_ids.Add(user_id);
    }
    public void remove_permission(string user_id) {
        user_ids.Remove(user_id);
    }
    public string get_id(){
        return id;
    }
    public void addProcess(Process process)
    {
        handler.add_process(process);
    }
    public void set_inventory(string inventory, List<Machine> machines)
    {
        this.inventory = inventory;
        this.machines = machines;
    }
    public List<Machine> get_machines() {
        return machines;
    }
    public void add_subset_inventory(string name, string subset_inventories)
    {
        this.subset_inventories.Add(new KeyValuePair<string, string>(name, subset_inventories));
    }
    public string get_inventory()
    {
        return inventory;
    }
    public string get_inventory(string name)
    {
        KeyValuePair<string, string> key_pair = subset_inventories.Find(x => x.Key == name);
        if (key_pair.Equals(null)) {
            return "";
        }
        return key_pair.Value;
    }
    public dynamic jobs_result(){
        return handler.parse_processes();
    }
    public void set_directory(string dir) {
        if (!foldername.IsNullOrEmpty()) {
            try{
                Directory.Delete($@"Resource/files/{foldername}", true);
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
            }
        }
        foldername = dir;
    }
    public string get_directory() {
        return foldername;
    }
    public void set_pull_directory(string dir) {
        if (!pulled_foldername.IsNullOrEmpty()) {
            try{
                Directory.Delete($@"Resource/files/{foldername}", true);
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
            }
        }
        pulled_foldername = dir;
    }
    public string get_pull_directory() {
        return pulled_foldername;
    }
}