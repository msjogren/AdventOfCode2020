using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

IEnumerable<(Opcode op, int arg)> Parse(string[] program)
{
    foreach (string line in program)
    {
        var parts = line.Split(' ');
        Opcode op = parts[0] switch {
            "acc" => Opcode.Acc,
            "jmp" => Opcode.Jmp,
            _     => Opcode.Nop
        };

        yield return (op, int.Parse(parts[1]));
    }
}

(bool completed, int result) Run((Opcode op, int arg)[] code)
{
    int ip = 0, acc = 0;
    var visited = new bool[code.Length];

    int Accumulate(int arg) { acc += arg; return 1; }

    while (ip < code.Length && !visited[ip]) {
        visited[ip] = true;

        ip += code[ip].op switch {
            Opcode.Nop => 1,
            Opcode.Acc => Accumulate(code[ip].arg),
            Opcode.Jmp => code[ip].arg
        };
    }

    return (ip >= code.Length, acc);
}

var input = File.ReadAllLines("input.txt");
var code = Parse(input).ToArray();

// Part 1
var part1Result = Run(code);
Console.WriteLine(part1Result.result);

// Part 2
for (int changeIp = 0; changeIp < code.Length; changeIp++)
{
    if (code[changeIp].op != Opcode.Jmp && code[changeIp].op != Opcode.Nop) continue;
    var oldOp = code[changeIp].op;
    code[changeIp].op = oldOp == Opcode.Jmp ? Opcode.Nop : Opcode.Jmp;

    var part2Result = Run(code);
    code[changeIp].op = oldOp;

    if (part2Result.completed) Console.WriteLine(part2Result.result);
}

enum Opcode
{
    Nop,
    Acc,
    Jmp
}

