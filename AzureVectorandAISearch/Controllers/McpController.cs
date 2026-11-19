//using Azure.Identity;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc;
//using Azure.Identity;
//using Azure.ResourceManager;
//using Azure.ResourceManager.Compute;
//using Azure.ResourceManager.Storage;

//namespace AzureVectorandAISearch.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class McpController : ControllerBase
//    {
//        private readonly ArmClient _armClient;

//        public McpController()
//        {
//            // Authenticate to Azure using DefaultAzureCredential
//            _armClient = new ArmClient(new DefaultAzureCredential());
//        }

//        // List all Storage Accounts
//        [HttpPost("list-storage")]
//        public async Task<IActionResult> ListStorageAccounts()
//        {
//            var storageAccounts = new List<string>();
//            var subscription = await _armClient.GetDefaultSubscriptionAsync();

//            await foreach (var sa in subscription.GetStorageAccounts().GetAllAsync())
//            {
//                storageAccounts.Add(sa.Data.Name);
//            }

//            return Ok(storageAccounts);
//        }

//        // Start or Stop a VM
//        [HttpPost("vm-action")]
//        public async Task<IActionResult> VmAction([FromBody] VmActionRequest request)
//        {
//            var subscription = await _armClient.GetDefaultSubscriptionAsync();
//            var vm = await subscription.GetVirtualMachineAsync(request.ResourceGroupName, request.VmName);

//            if (request.Action.ToLower() == "start")
//                await vm.Value.StartAsync();
//            else if (request.Action.ToLower() == "stop")
//                await vm.Value.PowerOffAsync();
//            else
//                return BadRequest("Invalid action. Use 'start' or 'stop'.");

//            return Ok($"VM '{request.VmName}' has been {request.Action}ed.");
//        }
//    }

//    public class VmActionRequest
//    {
//        public string ResourceGroupName { get; set; }
//        public string VmName { get; set; }
//        public string Action { get; set; }
//    }
//}
