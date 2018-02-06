module Fable.Import.FileInput
open Fable.Core.DynamicExtensions
open Fable.Import
open Fable.Core
open JsInterop

type FileInfo<'t> = 
    { Name: string; MIME: string; Data: 't }

[<Emit("new Promise($0)")>]
let createPromise (executor: ('t -> unit) -> (exn -> unit) -> unit): JS.Promise<'t> = jsNative

[<Emit("$1.then($0)")>]
let consumePromise (callback: 't->unit) (promise: JS.Promise<'t>): unit = jsNative
let private readInternal<'t> (readMethod: string) (blob: Browser.Blob) = 
    createPromise(fun resolve reject -> 
        try
            let reader = Browser.FileReader.Create()
            reader.addEventListener("loadend", !^(fun _ -> 
                match reader.error with
                | null ->  
                    reader.result |> unbox<'t> |> resolve
                | _ -> 
                    reader.error.["message"] |> string |> exn |> reject)          
            )
            reader.[readMethod].Invoke(blob) |> ignore
        with 
        | e -> e |> reject
    )
let readAsText blob: JS.Promise<string> = 
    readInternal "readAsText" blob
let readAsDataURL blob: JS.Promise<string> = 
    readInternal "readAsDataURL" blob
let readAsArrayBuffer blob: JS.Promise<JS.ArrayBuffer> = 
    readInternal "readAsArrayBuffer" blob

module React = 
    open Fable.Helpers.React
    open Props
    let extract f list = 
        let rec seek traversed = function
            | h::t ->
                match f h with
                | Some h' -> Some h', (List.rev traversed)@t
                | _ -> seek (h::traversed) t
            | [] -> None, (List.rev traversed)
        seek [] list

    type FileCallback = 
        | OnFileBytesReceived of (FileInfo<JS.ArrayBuffer> -> unit)
        | OnDataUrlReceived of (FileInfo<string> -> unit)
        | OnTextReceived of (FileInfo<string> -> unit)
        interface Props.IHTMLProp

    let inline singleFileInput (props: Props.IHTMLProp list) = 
        let existingChangeHandler, otherProps = 
            props |> extract (function 
                | :? DOMAttr as prop -> 
                    match prop with
                    | OnChange callback -> Some callback
                    | _ -> None
                | _ -> None)
        let loadCallback, withoutCallbacks = otherProps |> extract (function | :? FileCallback as fc -> Some fc | _ -> None)   
        let changeHandler (e: React.FormEvent) = 
            let files: Browser.FileList = !!e.target.["files"]
            if files.length > 0. then
                let file = files.[0]
                match loadCallback with
                | Some(OnFileBytesReceived r) -> 
                    readAsArrayBuffer file
                    |> consumePromise (fun bytes -> r { Name = file.name; MIME = file.``type``; Data = bytes })
                | Some(OnDataUrlReceived r) ->
                    readAsDataURL file
                    |> consumePromise (fun data -> r { Name = file.name; MIME = file.``type``; Data = data } )
                | Some(OnTextReceived r) ->
                    readAsText file
                    |> consumePromise (fun data -> r { Name = file.name; MIME = file.``type``; Data = data } )
                | _ -> Browser.console.warn("You probably need to attach callback to the file input field")                
            match existingChangeHandler with
            | Some h -> h e
            | _ -> ()
        input ([OnChange changeHandler; Type "file" ]@withoutCallbacks)