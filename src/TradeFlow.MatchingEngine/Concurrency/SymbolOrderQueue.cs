using System.Threading.Channels;
using TradeFlow.Domain.Entities;
using TradeFlow.MatchingEngine.Models;
using TradeFlow.MatchingEngine.Services;

namespace TradeFlow.MatchingEngine.Concurrency;

/// <summary>Serializes writes to one symbol while allowing separate symbols to execute in parallel.</summary>
public sealed class SymbolOrderQueue : IAsyncDisposable
{
    private readonly Channel<OrderCommand> _channel = Channel.CreateUnbounded<OrderCommand>(new UnboundedChannelOptions { SingleReader = true });
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Task _worker;

    public SymbolOrderQueue(string symbol)
    {
        Engine = new MatchingEngineService(symbol);
        _worker = RunAsync(_shutdown.Token);
    }

    public MatchingEngineService Engine { get; }

    public async Task<ExecutionReport> SubmitAsync(Order order, CancellationToken cancellationToken = default)
    {
        var completion = new TaskCompletionSource<ExecutionReport>(TaskCreationOptions.RunContinuationsAsynchronously);
        await _channel.Writer.WriteAsync(new OrderCommand(order, completion), cancellationToken);
        return await completion.Task.WaitAsync(cancellationToken);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        await foreach (var command in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                command.Completion.TrySetResult(Engine.Process(command.Order));
            }
            catch (Exception exception)
            {
                command.Completion.TrySetException(exception);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();
        _shutdown.Cancel();
        try { await _worker; } catch (OperationCanceledException) { }
        _shutdown.Dispose();
    }
}
