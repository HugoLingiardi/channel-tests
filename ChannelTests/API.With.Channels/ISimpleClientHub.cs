namespace API.With.Channels;

public interface ISimpleClientHub
{
    Task Ping(Data data);
}