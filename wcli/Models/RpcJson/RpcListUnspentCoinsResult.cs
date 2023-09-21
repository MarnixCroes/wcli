using System.Collections.Generic;
using System.Text.Json.Serialization;
using WasabiCli.Models.WalletWasabi;

namespace WasabiCli.Models.RpcJson;

public class RpcListUnspentCoinsResult : Rpc
{
    [JsonPropertyName("result")]
    [JsonRequired]
    public List<CoinInfo>? Result { get; set; }
}