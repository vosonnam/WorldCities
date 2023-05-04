import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, AsyncValidatorFn, AbstractControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../environments/environment';
import { Country } from './country';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.scss']
})
export class CountryEditComponent implements OnInit {

  title!: string;

  form!: FormGroup;

  country?: Country;

  id?: number;

  countries?: Country[];

  constructor(
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private activatedRoute: ActivatedRoute,
    private router:Router

  ) { }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: ['',
        [
          Validators.required
        ],
        this.isDupeField('name')
      ],
      iso2: ['',
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z]{2}$')
        ],
        this.isDupeField('iso2')
      ],
      iso3: ['',
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z]{3}$')
        ],
        this.isDupeField('iso3')
      ]
    });
  }

  loadData() {
    var paramId = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = paramId ? +paramId : 0;
    if (this.id) {
      var url = environment.BaseUrl + 'api/Countries/' + this.id;
      this.http.get<Country>(url).subscribe(
        result => {
          this.country = result;
          this.title = "Edit - " + this.country.name;
          this.form.patchValue(this.country);
        },
        error => console.error(error)
      );
    } else {
      this.title = "Create a new Country";
    }
  }

  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};
    if (country) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;

      if (this.id) {
        var url = environment.BaseUrl + 'api/Countries/' + this.id;
        this.http.put<Country>(url, country).subscribe(
          result => {
            console.log("Country " + country!.id + " has been updated.");
            this.router.navigate(['/countries']);
          },
          error => console.error(error)
        );
      }
    } else {
      var url = environment.BaseUrl + 'api/Countries';
      this.http.post<Country>(url, country).subscribe(
        result => {
          console.log("Country " + result.id + " has been created.");
          this.router.navigate(['/countries']);
        },
        error => console.error(error)
      );
    }
  }

  isDupeField(fieldName:string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null>=>{
      var url = environment.BaseUrl + 'api/Countries/isDupeField';
      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);
      return this.http.post<boolean>(url, params).pipe(map(result => {
        return result ? { isDupeField:true} : null;
      }));
    }
  }

}
