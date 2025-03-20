namespace Core.Interfaces;

public interface ICommand<out T>
{
    T Execute();
}

public interface ICommand
{
    void Execute();
}