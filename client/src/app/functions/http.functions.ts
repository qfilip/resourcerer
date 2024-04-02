import { HttpErrorResponse } from "@angular/common/http";

export function onHttp(error: HttpErrorResponse, actions: {
    onBadRequest?: () => void,
    onNotFound?: () => void,
    onRejected?: () => void,
    onError?: () => void,
    onAny?: () => void
}) {
    if(error.status === 400) {
        invokeNullable(actions.onBadRequest);
    }
    else if(error.status === 404) {
        invokeNullable(actions.onNotFound);
    }
    else if(error.status === 409) {
        invokeNullable(actions.onRejected);
    }
    else {
        invokeNullable(actions.onError);
    }

    invokeNullable(actions.onAny);
}

function invokeNullable(fn?: () => void) {
    if(fn) {
        fn();
    }
}