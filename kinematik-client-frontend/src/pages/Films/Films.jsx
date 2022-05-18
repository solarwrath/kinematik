import React, { useEffect, useState } from 'react';

import classes from './Films.module.css';

import { Swiper, SwiperSlide } from 'swiper/react';
import { Pagination, A11y, Keyboard, Mousewheel } from 'swiper';

import 'swiper/css';
import 'swiper/css/pagination';

import { Link } from 'react-router-dom';

const Films = (props) => {
    const [runningFilms, setRunningFilms] = useState([]);

    useEffect(async () => {
        const rawResponse = await fetch(window.apiBaseUrl + '/films/running');
        const parsedResponse = await rawResponse.json();
        const mappedRunningFilms = parsedResponse.runningFilms.map(
            (runningFilm) => {
                return {
                    filmId: runningFilm.id,
                    title: runningFilm.title,
                    posterUrl: runningFilm.posterUrl,
                };
            }
        );

        setRunningFilms(mappedRunningFilms);
    }, []);

    return (
        <div className={classes['films-page']}>
            <div className={classes['poster-carousel']}>
                <Swiper
                    modules={[Pagination, Keyboard, Mousewheel, A11y]}
                    spaceBetween={20}
                    slidesPerView={4}
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
                    {runningFilms.map((film) => (
                        <SwiperSlide key={film.filmId}>
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
