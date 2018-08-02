<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityFilter.ascx.vb" Inherits="UserControls_HospitalityFilter" %>


<!-- Please sanitize JS and add programmatically. -->
<script src="../../JavaScript/Module/Hospitality/HospitalityFixtures.js"></script>


<aside>
	<div class="c-hosp-fltr">
		<div class="row">
			<div class="column">
				<h1>Find The Perfect Hospitality Package&hellip;</h1>
				<div class="button-group c-hosp-fltr__s">
					<a href="#" class="button c-hosp-fltr__s-opp" data-toggle="c-hosp-fltr__s-opp">Opposition <i class="fa fa-angle-down" aria-hidden="true"></i></a>
					<div class="dropdown-pane dropdown-pane--light c-hosp-fltr__s-opp-pan" id="c-hosp-fltr__s-opp" data-dropdown data-hover="true" data-hover-pane="true">
						<ul class="menu vertical">
							<li><a href="#">Gillingham</a></li>
							<li><a href="#">Millwall</a></li>
							<li><a href="#">Bristol Rovers</a></li>
							<li><a href="#">Bradford</a></li>
						</ul>
					</div>
					<input type="text" id="c-hosp-fltr__datepicker" class="c-hosp-fltr__datepicker">
					<span class="button c-hosp-fltr__s-d"><i class="fa fa-calendar" aria-hidden="true"></i> Choose Date</span>
				</div>
				<div class="row c-hosp-fltr__sliders">
					<div class="columns c-hosp-fltr__lab c-hosp-fltr__lab-bud">
						<label for="c-hosp-fltr__bud-tb">What is your budget?</label>
					</div>
					<div class="columns c-hosp-fltr__slider c-hosp-fltr__slider-bud">
						<div class="c-slider">
	                        <span id="c-slider__slider" class="c-slider__slider"></span>
	                        <input id="c-hosp-fltr__bud-tb" name="c-hosp-fltr__bud-tb" type="text" value="50.00" id="" class="c-slider__tb" />
	                    </div>
					</div>
					<div class="columns c-hosp-fltr__lab c-hosp-fltr__lab-sz">
						<label for="c-hosp-fltr__sz-tb">What is your group size?</label>
					</div>
					<div class="columns c-hosp-fltr__slider c-hosp-fltr__slider-sz">
						<div class="c-slider">
	                        <span id="c-slider__slider" class="c-slider__slider"></span>
	                        <input id="c-hosp-fltr__sz-tb" name="c-hosp-fltr__sz-tb" type="text" value="50.00" id="" class="c-slider__tb" />
	                    </div>
					</div>
				</div>
				<div class="c-hosp-fltr__srch">
					<a href="#" class="button">Search <i class="fa fa-chevron-right" aria-hidden="true"></i></a>
				</div>
			</div>
		</div>
	</div>
</aside>

