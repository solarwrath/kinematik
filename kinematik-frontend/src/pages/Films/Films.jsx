import React, { useEffect } from 'react';

import classes from './Films.module.css';

import { Swiper, SwiperSlide } from 'swiper/react';
import { Pagination, A11y, Keyboard, Mousewheel } from 'swiper';

import 'swiper/css';
import 'swiper/css/pagination';

import theBatmanPosterUrl from './posters/the_batman.jpg';
import unchartedPosterUrl from './posters/uncharted.jpg';
import turningRedPosterUrl from './posters/turning_red.jpg';
import spiderManNoWayHomePosterUrl from './posters/spider_man_no_way_home.jpg';
import peacemakerPosterUrl from './posters/peacemaker.jpg';
import deathOnTheNilePosterUrl from './posters/death_on_the_nile.jpg';
import theAdamProjectPosterUrl from './posters/the_adam_project.jpg';
import { Link } from 'react-router-dom';

const demoFilms = [
    {
        filmId: 'the-batman',
        title: 'The Batman',
        posterUrl: theBatmanPosterUrl,
    },
    {
        filmId: 'uncharted',
        title: 'Uncharted',
        posterUrl: unchartedPosterUrl,
    },
    {
        filmId: 'turning-red',
        title: 'Turning Red',
        posterUrl: turningRedPosterUrl,
    },
    {
        filmId: 'spiderman',
        title: 'Spider-Man: No Way Home',
        posterUrl: spiderManNoWayHomePosterUrl,
    },
    {
        filmId: 'peacemaker',
        title: 'Peacemaker',
        posterUrl: peacemakerPosterUrl,
    },
    {
        filmId: 'death_on_the_nile',
        title: 'Death on the Nile',
        posterUrl: deathOnTheNilePosterUrl,
    },
    {
        filmId: 'the-adam-project',
        title: 'The Adam Project',
        posterUrl: theAdamProjectPosterUrl,
    },
];

const Films = (props) => {
    useEffect(async () => {
        const response = await fetch(window.apiBaseUrl + '/test');
        const text = await response.text();
        console.log(text);
    }, []);

    return (
        <div className={classes['films-page']}>
            <div className={classes['poster-carousel']}>
                <Swiper
                    modules={[Pagination, Keyboard, Mousewheel, A11y]}
                    spaceBetween={20}
                    slidesPerView={4}
                    loop={true}
                    pagination={{
                        el: `.${classes['poster-carousel-navigation']}`,
                        clickable: true,
                        dynamicBullets: true,
                    }}
                    keyboard={{
                        enabled: true,
                    }}
                    mousewheel={{
                        invert: true,
                    }}
                >
                    {demoFilms.map((film) => (
                        <SwiperSlide key={film.title}>
                            <Link
                                to={`/films/${film.filmId}`}
                                className={classes['poster']}
                                style={{
                                    backgroundImage: `url('${film.posterUrl}')`,
                                }}
                            >
                                <div className={classes['poster-content']}>
                                    <span className={classes['poster-title']}>
                                        {film.title}
                                    </span>
                                </div>
                            </Link>
                        </SwiperSlide>
                    ))}
                </Swiper>
            </div>

            <nav className={classes['poster-carousel-navigation']}></nav>
        </div>
    );
};

export default Films;
