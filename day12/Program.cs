using System;
using System.IO;
using System.Linq;


var input = File.ReadAllLines("input.txt");
var instructions = input.Select(line => (action: line[0], val: int.Parse(line.Substring(1))));

// Part 1
var state = (x: 0, y: 0, dir: 90);
foreach (var instr in instructions)
{
    state = instr.action switch {
        'N' => (x: state.x, y: state.y - instr.val, dir: state.dir),
        'S' => (x: state.x, y: state.y + instr.val, dir: state.dir),
        'E' => (x: state.x + instr.val, y: state.y, dir: state.dir),
        'W' => (x: state.x - instr.val, y: state.y, dir: state.dir),
        'L' => (x: state.x, y: state.y, dir: (state.dir - instr.val + 360) % 360),
        'R' => (x: state.x, y: state.y, dir: (state.dir + instr.val) % 360),
        'F' when state.dir ==   0 => (x: state.x, y: state.y - instr.val, dir: state.dir),
        'F' when state.dir ==  90 => (x: state.x + instr.val, y: state.y, dir: state.dir),
        'F' when state.dir == 180 => (x: state.x, y: state.y + instr.val, dir: state.dir),
        'F' when state.dir == 270 => (x: state.x - instr.val, y: state.y, dir: state.dir),
        _ => throw new InvalidOperationException()
    };
}

Console.WriteLine($"x {state.x} y {state.y} manhattan {Math.Abs(state.x) + Math.Abs(state.y)}");

// Part 2
var state2 = (ship: (x: 0, y: 0), wp: (x: 10, y: -1));
foreach (var instr in instructions)
{
    state2 = instr.action switch {
        'N' => (ship: state2.ship, wp: (x: state2.wp.x, y: state2.wp.y - instr.val)),
        'S' => (ship: state2.ship, wp: (x: state2.wp.x, y: state2.wp.y + instr.val)),
        'E' => (ship: state2.ship, wp: (x: state2.wp.x + instr.val, y: state2.wp.y)),
        'W' => (ship: state2.ship, wp: (x: state2.wp.x - instr.val, y: state2.wp.y)),
        'F' => (ship: (x: state2.ship.x + instr.val * state2.wp.x, y: state2.ship.y + instr.val * state2.wp.y), wp: state2.wp),
        'L' when instr.val ==  90 => (ship: state2.ship, wp: (x: state2.wp.y, y: -state2.wp.x)),
        'L' when instr.val == 180 => (ship: state2.ship, wp: (x: -state2.wp.x, y: -state2.wp.y)),
        'L' when instr.val == 270 => (ship: state2.ship, wp: (x: -state2.wp.y, y: state2.wp.x)),
        'R' when instr.val ==  90 => (ship: state2.ship, wp: (x: -state2.wp.y, y: state2.wp.x)),
        'R' when instr.val == 180 => (ship: state2.ship, wp: (x: -state2.wp.x, y: -state2.wp.y)),
        'R' when instr.val == 270 => (ship: state2.ship, wp: (x: state2.wp.y, y: -state2.wp.x)),
        _ => throw new InvalidOperationException()
    };
}

Console.WriteLine($"x {state2.ship.x} y {state2.ship.y} manhattan {Math.Abs(state2.ship.x) + Math.Abs(state2.ship.y)}");
