module Program

open System
open Elmish.WPF;
open MyNamespace;

type User = {
    mutable IsSelected: bool
    // IsSelected: bool
    Name: string
}

type Model = {
    Count: int
    StepSize: int
    Names: string list
    Users: User list
}

let init () = {
    Count = 0
    StepSize = 1
    Names = [
        "foo"
        "bar"
        "puyo"
        "huga"
    ]
    Users = [
        { IsSelected = false; Name = "Alice" }
        { IsSelected = true; Name = "Bob" }
        { IsSelected = true; Name = "Charles" }
    ]
}

type Msg =
    | Increment
    | Decrement
    | SetStepSize of int
    // | Toggle of (User * bool)
    | Ignore

type SubMsg =
    | ToggleSelected of bool

let update msg m =
    match msg with
    | Increment -> { m with Count = m.Count + m.StepSize }
    | Decrement -> { m with Count = m.Count - m.StepSize }
    | SetStepSize x -> { m with StepSize = x }
    | Ignore -> m
    // | Toggle (user, v) ->
    //     { m with Users = m.Users |> List.map (fun u ->
    //                                             if user = u
    //                                             then subUpdate (ToggleSelected v) u
    //                                             else u) }

let subUpdate msg sm =
    match msg with
    | ToggleSelected x ->
        sm.IsSelected <- x
        sm
    // | ToggleSelected x -> { sm with IsSelected = x }

let bindings () =
    [
        "CounterValue" |> Binding.oneWay (fun m -> m.Count)
        "Increment" |> Binding.cmd (fun m -> Increment)
        "Decrement" |> Binding.cmd (fun m -> Decrement)
        "StepSize" |> Binding.twoWay (
            (fun m -> float m.StepSize),
            (fun newVal m -> int newVal |> SetStepSize))
        "Names" |> Binding.oneWaySeq (
            (fun m -> m.Names), (=), id)
        "UserList" |> Binding.subModelSeq (
            (fun m -> m.Users),
            (fun sm -> sm.Name),
            (fun _ -> Ignore),
            (fun () -> [
                "Name" |> Binding.oneWay (fun (_, user) -> user.Name)
                // "IsSelected" |> Binding.twoWay (
                //     (fun (_, user) -> user.IsSelected),
                //     (fun value (_, user) -> Toggle (user, value)))
                "IsSelected" |> Binding.twoWay (
                    (fun (_, user) -> user.IsSelected),
                    ToggleSelected)
            ])
        )
    ]

[<EntryPoint; STAThread>]
let main argv =
    Program.mkSimpleWpf init update bindings
    |> Program.withDebugTrace
    |> Program.runWindowWithConfig {
        ElmConfig.Default with LogConsole=true; LogTrace=true; Measure=true }
        (MainWindow ())
