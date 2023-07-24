type IHandler<'a, 'b> =
    abstract member Handle: 'a -> 'b

type IBaseDto = interface end

type Bar () = class end

let foo<'TReq, 'TResp when 'TReq :> IBaseDto and 'TResp :> Bar>
    (handler: IHandler<'TReq, 'TResp>) = failwith "Not implemented"