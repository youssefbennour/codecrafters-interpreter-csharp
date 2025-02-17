using Sharlox;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: ./your_program.sh tokenize <filename>");
    Environment.Exit(1);
}

string command = args[0];
string filename = args[1];

if (command != "tokenize")
{
    Console.Error.WriteLine($"Unknown command: {command}");
    Environment.Exit(1);
}

string fileContents = File.ReadAllText(filename);

Console.Error.WriteLine("Logs from your program will appear here!");

var tokenizer = new Tokenizer(fileContents);
tokenizer.Tokenize();
tokenizer.DisplayTokens();

if (tokenizer.HasFailed)
{
    Environment.Exit(65);
}
else
{
    Environment.Exit(0);
}


