import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, tap } from "rxjs";
import { PopupService } from "../popup.service";
import { onHttp } from "../../functions/http.functions";
import { IPopup } from "../../models/components/IPopup";

@Injectable({
    providedIn: 'root'
})
export class PopupInterceptor implements HttpInterceptor {
    private service = inject(PopupService);

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(
                tap({
                    error: (e: HttpErrorResponse) => {
                        const warn = (xs: string[]) => {
                            if(!xs) return;
                            
                            const errors = xs.map(x => ({ message: x, type: 'warning' } as IPopup));
                            this.service.many(errors);
                        }

                        onHttp(e, {
                            onBadRequest: () => warn(e.error),
                            onNotFound: () => warn(e.error),
                            onRejected: () => warn(e.error),
                            onError: () => this.service.error('Server went Cthulhu')
                        });
                    }
                })
            )
    }
}