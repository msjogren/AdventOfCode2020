using System;
using System.IO;
using System.Linq;

long EvaluateExpression(string expr, Func<string, long> calculator)
{
    int parenDepth = 0;
    int endParenPos = 0;
    
    for (int i = expr.Length - 1; i >= 0; i--)
    {
        if (expr[i] == ')')
        {
            parenDepth++;
            if (endParenPos == 0) endParenPos = i;
        }
        else if (expr[i] == '(' && --parenDepth == 0)
        {
            string left = i > 0 ? expr[..i] : "";
            string right = endParenPos < expr.Length - 1 ? expr[(endParenPos + 1)..] : "";
            expr = left + EvaluateExpression(expr[(i + 1)..endParenPos], calculator).ToString() + right;
            endParenPos = 0;
        }
    }

    return calculator(expr);
}

long CalculateLeftToRight(string expr) =>
    ("+" + expr)
        .Replace("+ ", "+")
        .Replace("* ", "*")
        .Split(' ')
        .Select(op => (@operator: op[0], operand: long.Parse(op[1..])))
        .Aggregate(0L, (res, op) => op.@operator == '*' ? res * op.operand : res + op.operand);


long CalculateAdditionBeforeMultiplication(string expr) =>
    expr.Split(" * ").Aggregate(1L, (product, factor) =>
        product * factor.Split(" + ").Aggregate(0L, (sum, addend) => sum + long.Parse(addend)));


var input = File.ReadAllLines("input.txt");

// Part 1
Console.WriteLine(input.Select(line => EvaluateExpression(line, CalculateLeftToRight)).Sum());

// Part 2
Console.WriteLine(input.Select(line => EvaluateExpression(line, CalculateAdditionBeforeMultiplication)).Sum());
