using System;
using System.Collections.Generic;
namespace CloudShell_Test.Models
{

    public class PublicKey
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public string Key { get; set; }
    }

    public class CloudShellEnviromentResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string DockerImage { get; set; }
        public string State { get; set; }
        public string SshUsername { get; set; }
        public int SshPort { get; set; }
        public string SshHost { get; set; }
        public List<PublicKey> PublicKeys { get; set; }
        public string WebHost { get; set; }
        public string Size { get; set; }
        public DateTime VmSizeExpireTime { get; set; }
        public List<int> WebPorts { get; set; }
    }
}