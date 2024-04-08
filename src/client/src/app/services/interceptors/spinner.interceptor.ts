import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, tap } from "rxjs";
import { SpinnerService } from "../spinner.service";

@Injectable({
    providedIn: 'root'
})
export class SpinnerInterceptor implements HttpInterceptor {
    private service = inject(SpinnerService);

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(
                tap({
                    next: (okEvent) => {
                        if (okEvent instanceof HttpResponse) {
                            this.service.hide();
                        }
                        else {
                            this.service.show();
                        }
                    },
                    error: _ => {
                        this.service.hide();
                    }
                })
            )
    }
}