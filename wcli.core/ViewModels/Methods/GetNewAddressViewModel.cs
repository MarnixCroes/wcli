using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WasabiCli.Models;
using WasabiCli.Models.App;
using WasabiCli.Models.Services;
using WasabiCli.Models.RpcJson;

namespace WasabiCli.ViewModels.Methods;

public partial class GetNewAddressViewModel : RpcMethodViewModel
{
    [ObservableProperty] private string? _walletName;

    [NotifyCanExecuteChangedFor(nameof(GetNewAddressCommand))]
    [ObservableProperty] 
    private string? _label;

    public GetNewAddressViewModel(IRpcServiceViewModel rpcService, INavigationService navigationService, string walletName)
    {
        RpcService = rpcService;
        NavigationService = navigationService;
        WalletName = walletName;
        Label = "Label";
    }

    private bool CanGetNewAddress()
    {
        return Label is not null 
               && Label.Length > 0;
    }

    [RelayCommand(CanExecute = nameof(CanGetNewAddress))]
    private async Task GetNewAddress()
    {
        var job = CreateJob();

        if (RpcService.BatchMode)
        {
            OnBatch(job);
            return;
        }

        var result = await RpcService.Send<RpcGetNewAddressResult>(job);
        if (result is RpcGetNewAddressResult { Result: not null } rpcGetNewAddressResult)
        {
            OnRpcSuccess(rpcGetNewAddressResult);
        }
        else if (result is RpcErrorResult { Error: not null } rpcErrorResult)
        {
            OnRpcError(rpcErrorResult);
        }
        else if (result is Error error)
        {
            OnError(error);
        }
    }

    protected override void OnRpcSuccess(Rpc rpcResult)
    {
        if (rpcResult is RpcGetNewAddressResult rpcGetNewAddressResult)
        {
            NavigationService.Clear();
            NavigationService.Navigate(rpcGetNewAddressResult.Result);
        }
    }

    public override Job CreateJob()
    {
        var requestBody = new RpcMethod
        {
            Method = "getnewaddress",
            Params = new []
            {
                Label
            }
        };

        var rpcServerUri = $"{RpcService.ServerPrefix}/{WalletName}";

        return new Job(requestBody, rpcServerUri);
    }
}
