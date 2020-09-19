module Program

open System
open Elmish.WPF;
open MyNamespace;

type Model = {
    Count: int
    StepSize: int
}

let init () = {
    Count = 0
    StepSize = 1
}

type Msg =
    | Increment
    | Decrement
    | SetStepSize of int

let update msg m =
    match msg with
    | Increment -> { m with Count = m.Count + m.StepSize }
    | Decrement -> { m with Count = m.Count - m.StepSize }
    | SetStepSize x -> { m with StepSize = x }

let bindings () =
    [
        "CounterValue" |> Binding.oneWay (fun m -> m.Count)
        "Increment" |> Binding.cmd (fun m -> Increment)
        "Decrement" |> Binding.cmd (fun m -> Decrement)
        "StepSize" |> Binding.twoWay (
            (fun m -> float m.StepSize),
            (fun newVal m -> int newVal |> SetStepSize)
        )
    ]
let emptyBindings () : Binding<Model, Msg> list = []

[<EntryPoint; STAThread>]
let main argv =
    Program.mkSimpleWpf init update bindings
    // Program.mkSimpleWpf init update emptyBindings
    |> Program.runWindow (MainWindow ())

//let main window =
//  Program.mkSimpleWpf (fun () -> init) update bindings
//  |> Program.runWindow window