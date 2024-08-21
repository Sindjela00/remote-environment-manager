using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

static class Ansible {
    public static bool pull_files_func(Session session, string inventory)
        {
            string dirname = Path.GetRandomFileName();
            session.set_pull_directory(dirname);
            var processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'randomdir={dirname}\' Resource/playbooks/pull_files.yml\"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return true;
            }
            return false;
            //return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
        public static bool build_image_func(Session session, string predmet, string image, string inventory)
        {
            var files = Directory.GetFiles(@"./Resource/Dockerimages");

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            string file = files.ToList().Find(files => files.Contains(image));
            if (file == null)
            {
                return false;
            //return BadRequest("Image not found");
            }
            var processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'name={predmet} dockerimage={image}\' Resource/playbooks/build_docker.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return true;
            }
            return false;
            //return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
        public static bool start_image_func(Session session, string predmet, string image, string inventory)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'name={predmet} dockerimage={image}\' Resource/playbooks/run_container.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process? process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return true;
            }
            return false;
            //return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }

        public static bool stop_container_func(Session session, string predmet, string inventory)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'name={predmet}\' Resource/playbooks/stop_container.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process? process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return true;
            }
            return false;
            //return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
        public static bool push_files_func(Session session, string directory, string inventory)
        {
            var processInfo = new ProcessStartInfo("bash",
                $"-c \"ansible-playbook -i Resource/inventory/{inventory} --extra-vars \'source={directory}\' Resource/playbooks/push_files.yml \"");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process();

            process.StartInfo = processInfo;
            if (process.Start())
            {
                session.addProcess(process);
                return true;
            }
            return false;
            //return BadRequest($"Error starting build {process.StandardError.ReadToEnd()}");
        }
}