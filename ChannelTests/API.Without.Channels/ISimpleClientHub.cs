namespace API.Without.Channels;

public interface ISimpleClientHub
{
    Task Ping(Data data);
}